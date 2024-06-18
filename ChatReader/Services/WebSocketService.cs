using System.Net.WebSockets;
using System.Text;
using ChatReader.Data;

namespace ChatReader.Services;

public class WebSocketService(UserData userData, IWebSocketClient webSocketClient)
{
    private readonly UserData _userData = userData;
    private readonly IWebSocketClient _webSocketClient = webSocketClient;

    public async Task HandleConnection(WebSocket webSocket)
    {
        var user = _userData.GetUser();
        var buffer = new byte[1024 * 4];

        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Close || user == null)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, result.CloseStatusDescription, CancellationToken.None);
            return;
        }

        while (webSocket.State == WebSocketState.Open)
        {
            var message = _webSocketClient.DequeueParsedMessage();
            if (!string.IsNullOrEmpty(message))
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                await webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}