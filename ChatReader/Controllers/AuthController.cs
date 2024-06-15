using System.Text.Json;
using ChatReader.Data;
using ChatReader.Dto;
using Microsoft.AspNetCore.Mvc;

namespace ChatReader.Controllers;

public class AuthController(IHttpClientFactory clientFactory, UserData user) : Controller
{
    private readonly UserData _user = user;
    private readonly IHttpClientFactory _httpClientFactory = clientFactory;

    [HttpGet]
    public async Task<ActionResult> Index([FromQuery] string code)
    {
        var user = _user.GetUser();
        if (!ModelState.IsValid || user == null)
        {
            return BadRequest(new { error = "Authentication failed" });
        }

        string clientId = user.UserId;
        string clientSecret = user.UserSecret;
        string redirectUrl = "http://localhost:5293/auth/";
        string requestUrl = "https://id.twitch.tv/oauth2/token";
        FormUrlEncodedContent form = new(new Dictionary<string, string>
        {
            {"client_id", clientId},
            {"client_secret", clientSecret},
            {"code", code},
            {"grant_type", "authorization_code"},
            {"redirect_uri", redirectUrl}
        });

        var httpClient = _httpClientFactory.CreateClient();
        var httpResponseMessage = await httpClient.PostAsync(requestUrl, form);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            string content = await httpResponseMessage.Content.ReadAsStringAsync();
            var twitchData = JsonSerializer.Deserialize<AccessTokenDto?>(content);
            var token = twitchData?.access_token;
            if (twitchData == null || string.IsNullOrEmpty(token))
            {
                return BadRequest(new { error = "Invalid token received" });
            }

            HttpContext.Session.SetString("Token", token);
            return Ok(new { msg = "Authenticated successfully" });
        }
        else
        {
            return BadRequest(new { error = "Error during authentication" });
        }
    }

    [HttpPost]
    public async Task<ActionResult> Index([FromBody] UserAuthDto user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { error = "User credentials missing" });
        }

        string url = "https://id.twitch.tv/oauth2/";
        string redirectUrl = "http://localhost:5293/auth/";
        string scope = "chat:read";
        string requestUrl = new($"{url}authorize?response_type=code&client_id={user.UserId}&redirect_uri={redirectUrl}&scope={scope}");

        var httpClient = _httpClientFactory.CreateClient();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            _user.SaveUser(new UserAuthDto(user.Nick, user.UserId, user.UserSecret));
            return Ok(new { msg = "Credentials saved", requestUrl });
        }
        else
        {
            return BadRequest(new { error = "Invalid user credentials" });
        }
    }
}
