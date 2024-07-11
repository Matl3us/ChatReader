namespace ChatReader.Core.Models.Dto
{
    public class CodeAuthDto
    {
        public required string Code { get; set; }
        public string? Scope { get; set; }
    }
}
