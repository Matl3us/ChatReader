using ChatReader.Core;
using ChatReader.Core.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatReader.Controllers
{
    [Authorize]
    [Route("api/ws")]
    public class WebSocketController(WSConnectionsManager wsConnectionsManager) : ControllerBase
    {
        private readonly WSConnectionsManager _connectionManager = wsConnectionsManager;

        public async Task Index()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var authUser = HttpContext.User;
                var id = authUser.FindFirst("Id")!;
                var nick = authUser.FindFirst("Nick")!;
                var token = authUser.FindFirst("Token")!;

                var user = new AuthUserDto()
                {
                    Id = id.Value,
                    Nick = nick.Value,
                    Token = token.Value,
                };

                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                if (!_connectionManager.TryCreateConnection(user))
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                }


                await _connectionManager.StartConnectionAsync(webSocket, user);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
