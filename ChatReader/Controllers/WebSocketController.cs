using ChatReader.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace ChatReader.Controllers
{
    [Authorize]
    [Route("api/ws")]
    public class WebSocketController(ConcurentWebSockets webSockets) : ControllerBase
    {
        private readonly ConcurentWebSockets _webSockets = webSockets;

        public async Task Index()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var authUser = HttpContext.User;
                var id = authUser.FindFirst("Id")!;

                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                if (!_webSockets.webSocketsDict.TryAdd(id.Value, webSocket))
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                }

                await Echo(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private static async Task Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(
                    new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                    receiveResult.MessageType,
                    receiveResult.EndOfMessage,
                    CancellationToken.None);

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }
    }
}
