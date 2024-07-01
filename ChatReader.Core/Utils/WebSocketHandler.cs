using System.Net.WebSockets;
using System.Text;

namespace ChatReader.Core.Utils
{
    public class WebSocketHandler
    {
        public static async Task HandleConnectionAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            var receiveTask = ReceiveMessagesAsync(webSocket, cancellationToken);
            var sendTask = SendMessagesAsync(webSocket, cancellationToken);
            await Task.WhenAll(receiveTask, sendTask);
        }

        private static async Task ReceiveMessagesAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            var buffer = new byte[1024];

            while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                do
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                    var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received message: {msg}");
                } while (!result.EndOfMessage);
            }
        }

        private static async Task SendMessagesAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var msg = DateTime.Now.ToString();
                var bytes = Encoding.UTF8.GetBytes(msg);
                await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken);
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}
