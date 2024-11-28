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

        private HttpRequestMessage PrepareMessage(HttpMethod method, string requestUri, string token)
        {
            string clientId = _config["ClientId"];
            var requestMessage = new HttpRequestMessage(method, requestUri);
            requestMessage.Headers.Add("Client-Id", clientId);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return requestMessage;
        }

        public async Task<UserInfoDto?> GetUserInfo(string token, string username)
        {
            string requestUrl = "https://api.twitch.tv/helix/users";
            var query = $"?login={username}&";
            var requestMessage = PrepareMessage(HttpMethod.Get, requestUrl + query, token);

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) { return null; }

            string content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TwitchDataWrapperDto<UserInfoDto>>(content);
            return data?.GetFirst();
        }

        public async Task<UserChatColorDto?> GetUserChatColor(string token, string username)
        {
            var userInfo = await GetUserInfo(token, username);
            if (userInfo == null) return null;

            string requestUrl = "https://api.twitch.tv/helix/chat/color";
            var query = $"?user_id={userInfo.id}&";
            var requestMessage = PrepareMessage(HttpMethod.Get, requestUrl + query, token);

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) { return null; }

            string content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TwitchDataWrapperDto<UserChatColorDto>>(content);
            return data?.GetFirst();
        }

        public async Task<List<BadgeSetDto>> GetGlobalChatBadges(string token)
        {
            string requestUrl = "https://api.twitch.tv/helix/chat/badges/global";
            var requestMessage = PrepareMessage(HttpMethod.Get, requestUrl, token);

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) { return []; }

            string content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TwitchDataWrapperDto<BadgeSetDto>>(content);
            if (data is null) return [];

            return data.GetAll();
        }

        public async Task<List<BadgeSetDto>> GetChannelChatBadges(string token, string username)
        {
            var userInfo = await GetUserInfo(token, username);
            if (userInfo == null) return [];

            string requestUrl = "https://api.twitch.tv/helix/chat/badges";
            var query = $"?broadcaster_id={userInfo.id}&";
            var requestMessage = PrepareMessage(HttpMethod.Get, requestUrl + query, token);

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) { return []; }

            string content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TwitchDataWrapperDto<BadgeSetDto>>(content);
            if (data is null) return [];

            return data.GetAll();
        }
    }
}
