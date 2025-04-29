using System.Net;
using TestProject.Gameteq.Server;

Console.Clear();

var server = new TcpChatServer();
await server.InitializeAsync(IPAddress.Parse("127.0.0.1"), 3333);