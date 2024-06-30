using ChatReader.Core.Models;

namespace ChatReader.Core.Interfaces
{
    public interface IAuthService
    {
        public Task<AuthUserDto?> AuthenticateAsync(string code);
    }
}
