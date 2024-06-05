using System.Net.WebSockets;
using System.Text;
using ChatReader;

var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, ".env");
DotEnv.Load(dotenv);

Console.WriteLine("What is your nick?");
string? nick = Console.ReadLine();
if (nick == null)
{
    Console.Error.WriteLine("Nick cannot be empty");
    return;
}

Authorization auth = new("https://id.twitch.tv/oauth2/", "http://localhost:3000/", "chat:read");
await auth.Authorize();
if (auth.Token == null)
{
    Console.Error.WriteLine("Invalid token");
    return;
}

Console.WriteLine("What channel do you want to join?");
string? channel = Console.ReadLine();
if (channel == null)
{
    Console.Error.WriteLine("Channel cannot be empty");
    return;
}

Console.Title = "ChatReader Client";

using (ClientWebSocket webSocket = new())
{
    try
    {
        await webSocket.ConnectAsync(new Uri("wss://irc-ws.chat.twitch.tv:443"), CancellationToken.None);
        var buffer = new byte[256];
        var sb = new StringBuilder();

        Console.WriteLine("Connection opened");
        await webSocket.SendAsync(Encoding.UTF8.GetBytes($"PASS oauth:{auth.Token}"), WebSocketMessageType.Text, true, CancellationToken.None);
        await webSocket.SendAsync(Encoding.UTF8.GetBytes($"NICK {nick}"), WebSocketMessageType.Text, true, CancellationToken.None);
        await webSocket.SendAsync(Encoding.UTF8.GetBytes($"JOIN #{channel}"), WebSocketMessageType.Text, true, CancellationToken.None);

        while (webSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult? result = null;
            do
            {
                result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
            } while (!result.EndOfMessage);

            var iRCMessage = Parser.ParseMessage(sb.ToString());
            Console.WriteLine(iRCMessage.ToString());
            sb.Clear();
        }
    }
    catch (Exception e)
    {
        Console.Error.WriteLine("Error has ocurred:\n" + e.Message + e.ToString());
        return;
    }
}