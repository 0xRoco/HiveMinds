namespace HiveMinds.DTO;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public string LoyaltyStatement { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
}