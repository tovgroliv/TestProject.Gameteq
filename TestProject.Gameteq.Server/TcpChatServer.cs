using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using TestProject.Gameteq.Common.Data;
using TestProject.Gameteq.Common.Enums;

namespace TestProject.Gameteq.Server;

internal sealed class TcpChatServer
{
    private TcpListener? _listener;
    private List<TcpClient> _clients = new List<TcpClient>();
    private readonly ConcurrentQueue<Message> _history = new();
    private readonly ConcurrentDictionary<TcpClient, User> _users = new();

    private CancellationTokenSource _cancellationToken = new();

    /// <summary>
    /// Initialization of the TCP server.
    /// </summary>
    /// <param name="ip">IP address of the server</param>
    /// <param name="port">Portof the server</param>
    public async Task InitializeAsync(IPAddress ip, int port)
    {
        try
        {
            _listener = new TcpListener(ip, port);
            _listener.Start();
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Server not started: {e.Message}");
            return;
        }

        Console.WriteLine($"Server started - {ip}:{port}. Press Ctrl+C to stop.");
        Console.CancelKeyPress += (sender, e) =>
        {
            Console.Clear();
            Console.WriteLine("Shutting down...");
            _cancellationToken.Cancel();
            e.Cancel = true;
        };

        await RunServerAsync(_cancellationToken.Token);
    }

    /// <summary>
    /// The main stream for connecting users.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task RunServerAsync(CancellationToken cancellationToken)
    {
        if (_listener == null)
        {
            return;
        }

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!_listener.Pending())
                {
                    await Task.Delay(100, cancellationToken);
                    continue;
                }

                TcpClient client = await _listener.AcceptTcpClientAsync();
                _clients.Add(client);
                Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");

                _ = HandleClientAsync(client, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Server cancellation requested.");
        }
        finally
        {
            _users.Clear();
            _listener.Stop();
        }
    }

    /// <summary>
    /// The main stream of listening and reading packets from the user. The timeout is 1 minute.
    /// </summary>
    /// <param name="client">Connected user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
        using (client)
        {
            NetworkStream stream = client.GetStream();

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    byte[] lengthBuffer = new byte[4];
                    if (!await TryRead(lengthBuffer, stream, cancellationToken))
                    {
                        break;
                    }

                    int length = BitConverter.ToInt32(lengthBuffer, 0);
                    byte[] buffer = new byte[length];
                    if (!await TryRead(buffer, stream, cancellationToken))
                    {
                        break;
                    }

                    var receivedText = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                    var receive = JsonSerializer.Deserialize<ReceiveBase>(receivedText);

                    if (receive?.Type == ReceiveTypeEnum.Message)
                    {
                        var message = JsonSerializer.Deserialize<Message>(receivedText);
                        if (message == null)
                        {
                            continue;
                        }

                        PushHistory(message);
                        await Broadcast(message, cancellationToken);
                    }
                    else if (receive?.Type == ReceiveTypeEnum.SignUp)
                    {
                        var signup = JsonSerializer.Deserialize<Signup>(receivedText);
                        if (signup == null || !_users.TryAdd(client, signup.User))
                        {
                            continue;
                        }

                        foreach (var message in _history)
                        {
                            await Send(message, stream, cancellationToken);
                        }
                        await Broadcast(new UserStatus(signup.User, true), cancellationToken);
                    }
                    else if (receive?.Type == ReceiveTypeEnum.UserStatus)
                    {
                        var status = JsonSerializer.Deserialize<UserStatus>(receivedText);
                        await Broadcast(status, cancellationToken);
                    }
                    else
                    {
                        Console.WriteLine($"Wrong packet type: {client.Client.RemoteEndPoint}.");
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Client handler cancelled: {client.Client.RemoteEndPoint}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client error: {ex.Message}");
            }
            finally
            {
                Console.WriteLine($"Client disconnected: {client.Client.RemoteEndPoint}");
                if (_users.TryGetValue(client, out var user))
                {
                    await Broadcast(new UserStatus(user, false), cancellationToken);
                    _users.Remove(client, out _);
                }
                _clients.Remove(client);
            }
        }
    }

    /// <summary>
    /// Trying to read a set of bytes from a stream.
    /// </summary>
    /// <param name="buffer">Buffer for packet</param>
    /// <param name="stream">Running connection with user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>If true then reading is successful, otherwise if false then the server interrupted the connection</returns>
    private async Task<bool> TryRead(byte[] buffer, NetworkStream stream, CancellationToken cancellationToken)
    {
        var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(60));

        var byteCount = await stream.ReadAsync(buffer, 0, buffer.Length, timeout.Token);
        return byteCount != 0;
    }

    /// <summary>
    /// Add message to history. Message limit is 10.
    /// </summary>
    /// <param name="message">User message</param>
    private void PushHistory(Message message)
    {
        _history.Enqueue(message);
        while (_history.Count > 10)
        {
            _history.TryDequeue(out _);
        }
    }

    /// <summary>
    /// Send a packet to the user.
    /// </summary>
    /// <typeparam name="T">Packet type</typeparam>
    /// <param name="data">Packet instance</param>
    /// <param name="stream">Running connection with user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task Send<T>(T data, NetworkStream stream, CancellationToken cancellationToken)
    {
        var response = JsonSerializer.Serialize<T>(data);
        var responseBytes = Encoding.UTF8.GetBytes(response);
        var responseLength = BitConverter.GetBytes(response.Length);

        await stream.WriteAsync(responseLength, 0, responseLength.Length, cancellationToken);
        await stream.WriteAsync(responseBytes, 0, responseBytes.Length, cancellationToken);
    }

    /// <summary>
    /// Send a packet to all users.
    /// </summary>
    /// <typeparam name="T">Packet type</typeparam>
    /// <param name="data">Packet instance</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task Broadcast<T>(T data, CancellationToken cancellationToken)
    {
        foreach (var user in _users)
        {
            var stream = user.Key.GetStream();
            await Send<T>(data, stream, cancellationToken);
        }
    }
}

