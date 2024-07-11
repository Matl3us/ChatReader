using ChatReader.Core.Models.Dto;

namespace ChatReader.Core.Interfaces
{
    public interface IAuthService
    {
        public Task<AuthUserDto?> AuthenticateAsync(string code);
    }
}
