using ChatReader.Dto;
using ChatReader.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatReader.Controllers;

public class MessageController(IWebSocketClient webSocket) : Controller
{
    private readonly IWebSocketClient _websocketClient = webSocket;

    [HttpPost]
    public ActionResult Index([FromBody] MessageDto message)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { error = "Message cannot be empty" });
        }

        _websocketClient.SendMessage(message.Message, CancellationToken.None);
        return Ok(new { error = "Message sent" });
    }
}