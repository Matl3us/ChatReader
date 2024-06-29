using ChatReader.Core.Interfaces;
using ChatReader.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatReader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpGet]
        public async Task<ActionResult> Index([FromQuery] CodeAuthDto codeAuth)
        {
            var tokenAuth = await _authService.Authenticate(codeAuth.Code);
            if (tokenAuth == null)
            {
                return BadRequest(new { error = "Authentication failed" });
            }
            return Ok(new { msg = "Authenticated successfully" });
        }
    }
}
