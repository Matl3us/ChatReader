using ChatReader.Core.Models;
using ChatReader.Core.Models.Dto;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace ChatReader.Core.Utils
{
    public class IRCHandler
    {
        public static async Task<ClientWebSocket> StartConnectionAsync(CancellationToken cancellationToken)
        {
            var clientSocket = new ClientWebSocket();
            await clientSocket.ConnectAsync(new Uri("wss://irc-ws.chat.twitch.tv:443"), cancellationToken);
            return clientSocket;
        }

        public static async Task ReceiveMessagesAsync(ClientWebSocket webSocket, ConcurrentQueue<ParsedIRCMessage> msgQueue, AuthUserDto authUser, CancellationToken cancellationToken)
        {
            var buffer = new byte[1024];
            var sb = new StringBuilder();

            await SendMessageAsync(webSocket, $"PASS oauth:{authUser.Token}", cancellationToken);
            await SendMessageAsync(webSocket, $"NICK {authUser.Nick}", cancellationToken);

            while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                WebSocketReceiveResult? result = null;
                do
                {
                    result = await webSocket.ReceiveAsync(buffer, cancellationToken);
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                } while (!result.EndOfMessage);

                string[] subs = sb.ToString().Trim().Split("\r\n");
                sb.Clear();

                foreach (var sub in subs)
                {
                    var parsedMsg = IRCMessageParser.ParseMessage(sub);
                    msgQueue.Enqueue(parsedMsg);
                }
            }
        }

        public static async Task SendMessagesAsync(ClientWebSocket webSocket, ConcurrentQueue<string> queue, CancellationToken cancellationToken)
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
