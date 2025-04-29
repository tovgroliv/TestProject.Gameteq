using System.Net;
using TestProject.Gameteq.Server;

string ipAdressArg = args.Length > 0 ? args[0] : "127.0.0.1";
string portArg = args.Length > 1 ? args[1] : "3333";

if (!IPAddress.TryParse(ipAdressArg, out IPAddress? ipAddress))
{
    ipAddress = IPAddress.Parse("127.0.0.1");
}

if (!int.TryParse(portArg, out int port) || port < 1 || port > 65535)
{
    port = 3333;
}

Console.Clear();

var server = new TcpChatServer();
await server.InitializeAsync(ipAddress, port);