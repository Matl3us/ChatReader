using ChatReader.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatReader.Controllers;

public class WebSocketController(WebSocketService webSocketService) : ControllerBase
{
    private readonly WebSocketService _webSocketService = webSocketService;

    [Route("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await _webSocketService.HandleConnection(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}