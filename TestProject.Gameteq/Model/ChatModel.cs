using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using TestProject.Gameteq.Common.Data;
using TestProject.Gameteq.Common.Enums;
using TestProject.Gameteq.Common.Interfaces;

namespace TestProject.Gameteq.Model;

/// <summary>
/// Chat model in the MVP model.
/// </summary>
internal sealed class ChatModel()
{
    /// <summary>
    /// Message received from server
    /// </summary>
    public event Action<IMessage>? OnMessageReceived;
    /// <summary>
    /// User status change packet received.
    /// </summary>
    public event Action<IUserStatus>? OnUserStatusReceived;

    private NetworkStream? _stream;

    private User? user;

    private ConsoleColor[] _colors = [
        ConsoleColor.DarkRed, ConsoleColor.Red,
        ConsoleColor.DarkBlue, ConsoleColor.Blue,
        ConsoleColor.DarkCyan, ConsoleColor.Cyan,
        ConsoleColor.DarkGreen, ConsoleColor.Green,
        ConsoleColor.DarkCyan, ConsoleColor.Cyan,
        ConsoleColor.DarkMagenta, ConsoleColor.Magenta,
        ConsoleColor.DarkYellow, ConsoleColor.Yellow,
    ];

    private CancellationToken _cancellationToken;

    /// <summary>
    /// Initializing the model & tcp server.
    /// </summary>
    /// <param name="serverIp">IP address of the server</param>
    /// <param name="serverPort">Port of the server</param>
    /// <param name="username">Current user's nickname</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task Initialize(string serverIp, int serverPort, string username, CancellationToken cancellationToken)
    {
        user = new User(username, _colors[Random.Shared.Next(0, _colors.Length)]);

        _cancellationToken = cancellationToken;

        var client = new TcpClient();
        await client.ConnectAsync(serverIp, serverPort);
        await Receive(client, user);
    }

    /// <summary>
    /// Send message to server.
    /// </summary>
    /// <param name="text">Message text</param>
    public void SendMessage(string text)
    {
        if (user == null)
        {
            return;
        }

        Send(new Message(text, DateTime.Now, user));
    }

    /// <summary>
    /// The main stream of receiving packets from the server.
    /// </summary>
    /// <param name="client">Connected TCP client</param>
    /// <param name="user">Current user</param>
    private async Task Receive(TcpClient client, User user)
    {
        if (client == null)
        {
            return;
        }

        _stream = client.GetStream();

        try
        {
            Send(new Signup(user));

            while (!_cancellationToken.IsCancellationRequested)
            {
                byte[] lengthBuffer = new byte[4];
                if (!await TryRead(lengthBuffer, _stream, _cancellationToken))
                {
                    break;
                }

                int length = BitConverter.ToInt32(lengthBuffer, 0);
                byte[] buffer = new byte[length];
                if (!await TryRead(buffer, _stream, _cancellationToken))
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

                    OnMessageReceived?.Invoke(message);
                }
                else if (receive?.Type == ReceiveTypeEnum.UserStatus)
                {
                    var status = JsonSerializer.Deserialize<UserStatus>(receivedText);
                    if (status == null)
                    {
                        continue;
                    }

                    OnUserStatusReceived?.Invoke(status);
                }
                else
                {
                    Console.WriteLine("Wrong packet type from server.");
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
            client.Close();
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
    /// Send a packet to the user.
    /// </summary>
    /// <typeparam name="T">Packet type</typeparam>
    /// <param name="data">Packet instance</param>
    public void Send<T>(T data)
    {
        if (_stream == null)
        {
            return;
        }

        var response = JsonSerializer.Serialize<T>(data);
        var responseBytes = Encoding.UTF8.GetBytes(response);
        var responseLength = BitConverter.GetBytes(response.Length);

        _stream.Write(responseLength, 0, responseLength.Length);
        _stream.Write(responseBytes, 0, responseBytes.Length);
    }
}
