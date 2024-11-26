using ChatReader.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatReader.Controllers
{
    [Authorize]
    [Route("api/user")]
    public class UserController(ITwitchHTTPService twitchService) : ControllerBase
    {
        private readonly ITwitchHTTPService _twichService = twitchService;

        [HttpGet("")]
        public async Task<IActionResult> Index([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("Username cannot be empty");
            }

            var authUser = HttpContext.User;
            var token = authUser.FindFirst("Token")!;
            var user = await _twichService.GetUserInfo(token.Value, username);

            if (user == null)
            {
                return BadRequest("Username doesn't exist");
            }

            return Ok(user);
        }

        [HttpGet("color")]
        public async Task<IActionResult> Color([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("Username cannot be empty");
            }

            var authUser = HttpContext.User;
            var token = authUser.FindFirst("Token")!;
            var user = await _twichService.GetUserChatColor(token.Value, username);

            if (user == null)
            {
                return BadRequest("Username doesn't exist");
            }

            return Ok(user);
        }
    }
}
