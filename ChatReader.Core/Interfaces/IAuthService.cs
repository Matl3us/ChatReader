using ChatReader.Core.Models;

namespace ChatReader.Core.Interfaces
{
    public interface IAuthService
    {
        public Task<TokenAuthDto?> Authenticate(string code);
    }
}
