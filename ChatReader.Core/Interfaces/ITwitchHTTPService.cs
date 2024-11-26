using ChatReader.Core.Models.Dto;

namespace ChatReader.Core.Interfaces
{
    public interface ITwitchHTTPService
    {
        public Task<UserInfoDto?> GetUserInfo(string token, string username);
    }
}
