using ChatReader.Core.Models;
using ChatReader.Core.Services;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace ChatReader.Core.Utils
{
    public static class TwitchIRCHandler
    {
        public static async Task StartConnectionAsync(TwitchIRCConnection twitchIRCConnection, CancellationToken cancellationToken)
        {
            var clientSocket = twitchIRCConnection.ClientWebSocket;
            await clientSocket.ConnectAsync(new Uri("wss://irc-ws.chat.twitch.tv:443"), cancellationToken);
        }

        public static async Task ReceiveMessagesAsync(TwitchIRCConnection twitchIRCConnection, AuthUserDto authUser, CancellationToken cancellationToken)
        {
            var clientSocket = twitchIRCConnection.ClientWebSocket;
            var messageQueue = twitchIRCConnection.MessageQueue;

            var buffer = new byte[1024];
            var sb = new StringBuilder();

            await SendMessageAsync(clientSocket, $"PASS oauth:{authUser.Token}", cancellationToken);
            await SendMessageAsync(clientSocket, $"NICK {authUser.Nick}", cancellationToken);

            while (clientSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                WebSocketReceiveResult? result = null;
                do
                {
                    result = await clientSocket.ReceiveAsync(buffer, cancellationToken);
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                } while (!result.EndOfMessage);

                string[] subs = sb.ToString().Trim().Split("\r\n");
                sb.Clear();

                foreach (var sub in subs)
                {
                    messageQueue.Enqueue(sub);
                }
            }
        }

        public static async Task SendMessagesAsync(TwitchIRCConnection twitchIRCConnection, ConcurrentQueue<string> queue, CancellationToken cancellationToken)
        {
            var webSocket = twitchIRCConnection.ClientWebSocket;
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

        public static async Task SendMessageAsync(ClientWebSocket webSocket, string message, CancellationToken cancellationToken)
        {
            if (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                await webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, cancellationToken);
            }
        }
    }
}
