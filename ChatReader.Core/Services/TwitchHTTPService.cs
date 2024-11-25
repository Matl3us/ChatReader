using ChatReader.Core.Interfaces;
using ChatReader.Core.Models.Dto;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ChatReader.Core.Services
{
    public class TwitchHTTPService(HttpClient httpClient, IConfiguration config) : ITwitchHTTPService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _config = config;

        public async Task<UserListDto?> GetUserInfo(string token, string username)
        {
            string clientId = _config["ClientId"];

            string requestUrl = "https://api.twitch.tv/helix/users";
            var query = $"?login={username}&";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl + query);
            requestMessage.Headers.Add("Client-Id", clientId);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) { return null; }

            string content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
            return JsonSerializer.Deserialize<UserListDto>(content);
        }
    }
}
