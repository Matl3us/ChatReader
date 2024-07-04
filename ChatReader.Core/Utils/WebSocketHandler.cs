using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace ChatReader.Core.Utils
{
    public static class WebSocketHandler
    {
        public static async Task ReceiveMessagesAsync(WebSocket webSocket, ConcurrentQueue<string> queue, CancellationToken cancellationToken)
        {
            var buffer = new byte[1024];

            while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                do
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                    var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    queue.Enqueue(msg);
                } while (!result.EndOfMessage);
            }
        }

        public static async Task SendMessagesAsync(WebSocket webSocket, ConcurrentQueue<string> queue, CancellationToken cancellationToken)
        {
            while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                if (queue.TryDequeue(out var message))
                {
                    var bytes = Encoding.UTF8.GetBytes(message);
                    await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken);
                }
                else
                {
                    await Task.Delay(1000, cancellationToken);
                }
            }
        }
    }
}
