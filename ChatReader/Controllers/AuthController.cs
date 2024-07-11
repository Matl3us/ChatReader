using ChatReader.Core.Interfaces;
using ChatReader.Core.Models.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            var user = await _authService.AuthenticateAsync(codeAuth.Code);
            if (user == null)
            {
                return BadRequest(new { error = "Authentication failed" });
            }

            var claims = new List<Claim>
            {
                new("Id", user.Id),
                new("Nick", user.Nick),
                new("Token", user.Token),
            };
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return Redirect("http://localhost:5240/");
        }

        [Authorize]
        [HttpGet("check")]
        public ActionResult Check()
        {
            return Ok(new { msg = "Authorized successfully" });
        }
    }
}
