using ChatReader.Core.Interfaces;
using ChatReader.Core.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace ChatReader.Core.Services
{
    public class AuthService(HttpClient httpClient, IConfiguration config) : IAuthService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _config = config;

        public async Task<TokenAuthDto?> Authenticate(string code)
        {
            var respnseMessage = await SendRequestAsync(code);
            Console.WriteLine(respnseMessage.IsSuccessStatusCode);
            if (!respnseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("Exit");
                return null;
            }

            string content = await respnseMessage.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TokenAuthDto?>(content);
        }

        private async Task<HttpResponseMessage> SendRequestAsync(string code)
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
    }
}
