namespace ChatReader.Core.Models
{
    public class AuthUserDto
    {
        public required string Id { get; set; }
        public required string Nick { get; set; }
        public required string Token { get; set; }
    }
}
