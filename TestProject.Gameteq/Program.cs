using TestProject.Gameteq.Model;
using TestProject.Gameteq.Presenter;
using TestProject.Gameteq.View;

var model = new ChatModel();
var view = new ChatView();
var presenter = new ChatPresenter(view, model);
await presenter.Initialize();
