using ChatReader.Core.Services;
using ChatReader.Core.Utils;
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

                await WebSocketHandler.HandleConnectionAsync(webSocket, CancellationToken.None);
                _webSockets.webSocketsDict.TryRemove(id.Value, out WebSocket? tmp);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
