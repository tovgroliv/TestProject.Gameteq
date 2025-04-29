using System.Net;
using TestProject.Gameteq.Model;
using TestProject.Gameteq.View;

namespace TestProject.Gameteq.Presenter;

/// <summary>
/// Chat presenter in the MVP model.
/// </summary>
internal sealed class ChatPresenter
{
    private readonly ChatModel _chatModel;
    private readonly ChatView _chatView;

    private CancellationTokenSource _cancellationToken = new();

    /// <summary>
    /// Chat presenter ctor.
    /// </summary>
    /// <param name="view">Chat view</param>
    /// <param name="model">Chat model</param>
    public ChatPresenter(ChatView view, ChatModel model)
    {
        _chatView = view;
        _chatModel = model;
        
        _chatView.EnterMessage += _chatModel.SendMessage;
        _chatModel.OnMessageReceived += _chatView.ShowMessage;
        _chatModel.OnUserStatusReceived += _chatView.ShowUserStatus;
    }

    /// <summary>
    /// Initializing the presenter.
    /// </summary>
    /// <param name="ip">IP address of the server</param>
    /// <param name="port">Portof the server</param>
    public async Task Initialize(IPAddress ip, int port)
    {
        var username = _chatView.Initialize(_cancellationToken.Token);

        Console.CancelKeyPress += (sender, e) =>
        {
            _cancellationToken.Cancel();
            e.Cancel = true;
        };

        await _chatModel.Initialize(ip, port, username, _cancellationToken.Token);
    }
}
