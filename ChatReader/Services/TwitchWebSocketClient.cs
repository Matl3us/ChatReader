using System.Net.WebSockets;
using System.Text;
using ChatReader.Data;

namespace ChatReader.Services;

public class TwitchWebSocketClient(UserData user) : IWebSocketClient
{
    private readonly UserData _user = user;
    private readonly ClientWebSocket webSocket = new();
    private readonly Queue<string> _parsedMessages = new();

    public async void Start(string token, CancellationToken cancellationToken)
    {
        var user = _user.GetUser();
        if (user == null)
        {
            return;
        }

        try
        {
            await webSocket.ConnectAsync(new Uri("wss://irc-ws.chat.twitch.tv:443"), cancellationToken);
            var buffer = new byte[256];
            var sb = new StringBuilder();

            await SendMessage($"PASS oauth:{token}", cancellationToken);
            await SendMessage($"NICK {user?.Nick}", cancellationToken);

            while (webSocket.State == WebSocketState.Open)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                WebSocketReceiveResult? result = null;
                do
                {
                    result = await webSocket.ReceiveAsync(buffer, cancellationToken);
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                } while (!result.EndOfMessage);

                _parsedMessages.Enqueue(sb.ToString());
                sb.Clear();
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("Error has ocurred:\n" + e.Message + e.ToString());
            return;
        }
    }

    public async Task SendMessage(string message, CancellationToken cancellationToken)
    {
        if (webSocket.State != WebSocketState.Open)
        {
            return;
        }
        var bytes = Encoding.UTF8.GetBytes(message);
        await webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, cancellationToken);
    }

    public string? DequeueParsedMessage()
    {
        if (_parsedMessages.Count != 0)
        {
            return _parsedMessages.Dequeue();
        }
        return null;
    }

}