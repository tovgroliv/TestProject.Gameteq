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
    public async Task Initialize()
    {
        var username = _chatView.Initialize(_cancellationToken.Token);

        Console.CancelKeyPress += (sender, e) =>
        {
            Console.Clear();
            Console.WriteLine("Shutting down...");
            _cancellationToken.Cancel();
            e.Cancel = true;
        };

        await _chatModel.Initialize("127.0.0.1", 3333, username, _cancellationToken.Token);
    }
}
