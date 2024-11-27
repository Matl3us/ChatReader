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
    }
}
