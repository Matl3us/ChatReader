using System.Net.WebSockets;
using System.Text;
using ChatReader;

var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, ".env");
DotEnv.Load(dotenv);

string? token = Environment.GetEnvironmentVariable("TOKEN");
string? nick = Environment.GetEnvironmentVariable("NICK");
string? channel = Environment.GetEnvironmentVariable("CHANNEL");

if (token == null || nick == null || channel == null)
{
    Console.Error.WriteLine("Empty env variables");
    return;
}

Console.Title = "ChatReader Client";

using (ClientWebSocket client = new())
{
    try
    {
        await client.ConnectAsync(new Uri("wss://irc-ws.chat.twitch.tv:443"), CancellationToken.None);
        var buffer = new byte[256];

        Console.WriteLine("Connection opened");
        await client.SendAsync(Encoding.UTF8.GetBytes($"PASS oauth:{token}"), WebSocketMessageType.Text, true, CancellationToken.None);
        await client.SendAsync(Encoding.UTF8.GetBytes($"NICK {nick}"), WebSocketMessageType.Text, true, CancellationToken.None);
        await client.SendAsync(Encoding.UTF8.GetBytes($"JOIN #{channel}"), WebSocketMessageType.Text, true, CancellationToken.None);

        while (client.State == WebSocketState.Open)
        {
            var result = await client.ReceiveAsync(buffer, CancellationToken.None);
            Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, result.Count));
        }
    }
    catch (Exception e)
    {
        Console.Error.WriteLine("Error has ocurred:\n" + e.Message);
        return;
    }
}