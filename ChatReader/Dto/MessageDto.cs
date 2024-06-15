using System.ComponentModel.DataAnnotations;

namespace ChatReader.Dto;

public class MessageDto
{
    [Required]
    public string Message { get; set; } = "";
}