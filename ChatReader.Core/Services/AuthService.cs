using ChatReader.Core.Interfaces;
using ChatReader.Core.Models.Dto;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ChatReader.Core.Services
{
    public class AuthService(HttpClient httpClient, IConfiguration config) : IAuthService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _config = config;

        public async Task<AuthUserDto?> AuthenticateAsync(string code)
        {
            var tokenResponseMessage = await RequestTokenAsync(code);
            var tokenAuthData = await ParseTokenRequest(tokenResponseMessage);
            if (tokenAuthData == null) { return null; }

            var userNickResponseMessage = await RequestUserAsync(tokenAuthData.access_token);
            var userList = await ParseUserRequest(userNickResponseMessage);
            if (userList == null) { return null; }

            var user = userList.data[0];
            return new AuthUserDto()
            {
                Id = user.id,
                Nick = user.login,
                Token = tokenAuthData.access_token
            };
        }

        private async Task<HttpResponseMessage> RequestTokenAsync(string code)
        {
            string clientId = _config["ClientId"];
            string clientSecret = _config["ClientSecret"];
            string redirectUrl = "http://localhost:5240/api/auth/";
            string requestUrl = "https://id.twitch.tv/oauth2/token";
            FormUrlEncodedContent form = new(new Dictionary<string, string>
            {
                {"client_id", clientId},
                {"client_secret", clientSecret},
                {"code", code},
                {"grant_type", "authorization_code"},
                {"redirect_uri", redirectUrl}
            });

            return await _httpClient.PostAsync(requestUrl, form);
        }

        private async Task<HttpResponseMessage> RequestUserAsync(string token)
        {
            string clientId = _config["ClientId"];
            string requestUrl = "https://api.twitch.tv/helix/users";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            requestMessage.Headers.Add("Client-Id", clientId);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.SendAsync(requestMessage);
        }

        private static async Task<TokenAuthDto?> ParseTokenRequest(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode) { return null; }

            string content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TokenAuthDto?>(content);
        }

        private static async Task<UserListDto?> ParseUserRequest(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode) { return null; }

            string content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserListDto?>(content);
        }
    }
}
