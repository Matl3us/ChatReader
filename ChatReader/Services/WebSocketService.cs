using System.Net.WebSockets;
using ChatReader.Data;

namespace ChatReader.Services;

public class WebSocketService(UserData userData)
{
    private readonly UserData _userData = userData;
    private readonly List<WebSocket> _sockets = [];

    public async Task HandleConnection(WebSocket webSocket)
    {
        _sockets.Add(webSocket);

        var user = _userData.GetUser();
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close || user == null)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, result.CloseStatusDescription, CancellationToken.None);
                break;
            }

            foreach (var s in _sockets)
            {
                await s.SendAsync(buffer[..result.Count], WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
        _sockets.Remove(webSocket);
    }
}