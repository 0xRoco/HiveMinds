namespace HiveMinds.DTO;

public class LoginResponseDto
{
    public int UserId { get; set; }
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}