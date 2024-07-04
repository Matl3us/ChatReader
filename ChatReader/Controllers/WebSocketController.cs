using ChatReader.Core;
using ChatReader.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatReader.Controllers
{
    [Authorize]
    [Route("api/ws")]
    public class WebSocketController(WSConnectionsManager connectionsManager) : ControllerBase
    {
        private readonly WSConnectionsManager _connectionsManager = connectionsManager;

        public async Task Index()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var authUser = HttpContext.User;
                var id = authUser.FindFirst("Id")!;
                var nick = authUser.FindFirst("Nick")!;
                var token = authUser.FindFirst("Token")!;

                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                if (!_connectionsManager.TryAdd(id.Value, webSocket))
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                }

                var user = new AuthUserDto()
                {
                    Id = id.Value,
                    Nick = nick.Value,
                    Token = token.Value,
                };
                await _connectionsManager.StartConnectionAsync(webSocket, user);
                _connectionsManager.TryRemove(id.Value);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
