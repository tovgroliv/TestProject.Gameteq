using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using TestProject.Gameteq.Common.Interfaces;

namespace TestProject.Gameteq.View;

/// <summary>
/// Chat view in the MVP model.
/// </summary>
internal sealed class ChatView()
{
    /// <summary>
    /// The user entered a message to send.
    /// </summary>
    public Action<string>? EnterMessage;

    private int _consoleWidth => Console.WindowWidth;
    private int _consoleTopPosition => Console.GetCursorPosition().Top;

    private string _message = "";

    private object _inputLock = new();
    private ConcurrentQueue<List<IMessage>> Messages { get; set; } = new();

    /// <summary>
    /// Initializing the user interface.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User entered nickname</returns>
    public string Initialize(CancellationToken cancellationToken)
    {
        var username = "";

        Console.Clear();
        Console.WriteLine("Welcome to the chat.");

        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("Your username can consist of English letters and numbers and be from 5 to 20 characters long.");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("Enter your name > ");
            Console.ResetColor();
            username = Console.ReadLine();

            Console.Clear();

            if (!string.IsNullOrEmpty(username) && Regex.IsMatch(username, @"^[A-Za-z0-9]{5,20}$"))
            {
                break;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("The username does not meet the requirements.");
            Console.ResetColor();
            username = "";
        }

        CreateConsoleThreads(cancellationToken);

        return username;
    }

    /// <summary>
    /// Create console threads.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    private void CreateConsoleThreads(CancellationToken cancellationToken)
    {
        Thread chatThread = new Thread(() => ChatThread(cancellationToken));
        chatThread.IsBackground = true;
        chatThread.Name = "Chat Thread";
        chatThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
        chatThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
        chatThread.Start();

        Thread chatInputThread = new Thread(() => ChatInputThread(cancellationToken));
        chatInputThread.IsBackground = true;
        chatInputThread.Name = "Chat Input Thread";
        chatInputThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
        chatInputThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
        chatInputThread.Start();
    }

    /// <summary>
    /// Print the user status on the screen.
    /// </summary>
    /// <param name="status">User status</param>
    public void ShowUserStatus(IUserStatus status)
    {
        var message = $"{status.User.Name} {(status.Connected ? "connected" : "disconnected")}";
        var indent = _consoleWidth / 2 - message.Length / 2;

        lock (_inputLock)
        {
            Clear();
            Console.ForegroundColor = ConsoleColor.DarkGray;

            if (indent > 0)
            {
                Console.SetCursorPosition(indent, Console.GetCursorPosition().Top);
            }

            Console.WriteLine(message);
            Console.ResetColor();
            ShowDivider();
        }
    }

    /// <summary>
    /// Print the message on the screen.
    /// </summary>
    /// <param name="message">User message</param>
    public void ShowMessage(IMessage message)
    {
        var date = message.Time.ToString("M/d/yyyy hh:mm tt");
        var datePosition = _consoleWidth - date.Length;
        var topPosition = _consoleTopPosition;

        lock (_inputLock)
        {
            Clear();
            Console.ForegroundColor = message.User.Color;
            Console.Write(message.User.Name);
            Console.ForegroundColor = ConsoleColor.DarkGray;

            if (datePosition < message.User.Name.Length)
            {
                topPosition++;
            }

            Console.SetCursorPosition(datePosition, topPosition);
            Console.WriteLine(date);
            Console.ResetColor();
            Console.WriteLine(message.Text);
            ShowDivider();
        }
    }

    /// <summary>
    /// Template for divider.
    /// </summary>
    private void ShowDivider()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(new string('*', Console.WindowWidth));
        Console.ResetColor();
    }

    /// <summary>
    /// Template for user input.
    /// </summary>
    private void ShowInput()
    {
        Console.Write($"> {_message}");
    }

    /// <summary>
    /// Clear the current terminal line.
    /// </summary>
    private void Clear()
    {
        lock (_inputLock)
        {
            Console.Write('\r');
            Console.Write(new string(' ', Console.WindowWidth));
            Console.Write('\r');
        }
    }

    /// <summary>
    /// Thread for chat render.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    private void ChatThread(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Clear();

            if (Messages.IsEmpty)
            {
                lock (_inputLock)
                {
                    ShowInput();
                }

                Thread.Sleep(10);
                continue;
            }

            while (!Messages.IsEmpty)
            {
                if (Messages.TryDequeue(out var mes))
                {
                    mes.ForEach(ShowMessage);
                }
            }
        }
    }

    /// <summary>
    /// Thread for user input.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    private void ChatInputThread(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (Console.IsInputRedirected || !Console.KeyAvailable)
            {
                continue;
            }

            lock (_inputLock)
            {
                var keyInfo = Console.ReadKey(false);

                if (keyInfo.KeyChar >= '0' && keyInfo.KeyChar <= '9' ||
                    keyInfo.KeyChar >= 'A' && keyInfo.KeyChar <= 'Z' ||
                    keyInfo.KeyChar >= 'a' && keyInfo.KeyChar <= 'z' ||
                    keyInfo.KeyChar == ' ' || keyInfo.KeyChar == '.')
                {
                    _message += keyInfo.KeyChar;
                }
                else if (keyInfo.KeyChar == '\b' || keyInfo.KeyChar == 127)
                {
                    if (_message.Length > 0)
                    {
                        _message = _message[..^1];
                    }
                }
                else if (keyInfo.KeyChar == '\r')
                {
                    EnterMessage?.Invoke(_message);
                    _message = "";
                }
            }
        }
    }
}
