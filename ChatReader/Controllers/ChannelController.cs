using ChatReader.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatReader.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ChannelController(ITwitchHTTPService twitchService) : ControllerBase
    {
        private readonly ITwitchHTTPService _twichService = twitchService;

        [HttpGet("badges/global")]
        public async Task<IActionResult> GlobalBadges()
        {
            var authUser = HttpContext.User;
            var token = authUser.FindFirst("Token")!;
            var badgesList = await _twichService.GetGlobalChatBadges(token.Value);
            return Ok(badgesList);
        }

        [HttpGet("badges")]
        public async Task<IActionResult> ChannelBadges([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("Username cannot be empty");
            }

            var authUser = HttpContext.User;
            var token = authUser.FindFirst("Token")!;
            var badgesList = await _twichService.GetChannelChatBadges(token.Value, username);
            return Ok(badgesList);
        }
    }
}
