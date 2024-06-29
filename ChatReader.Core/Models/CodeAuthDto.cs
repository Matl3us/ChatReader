namespace ChatReader.Core.Models
{
    public class CodeAuthDto
    {
        public required string Code { get; set; }
        public string? Scope { get; set; }
    }
}
