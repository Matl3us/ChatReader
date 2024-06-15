using System.ComponentModel.DataAnnotations;

namespace ChatReader.Dto;

public class UserAuthDto(string nick, string userId, string userSecret)
{
    [Required]
    public string Nick { get; set; } = nick;
    [Required]
    public string UserId { get; set; } = userId;
    [Required]
    public string UserSecret { get; set; } = userSecret;
}