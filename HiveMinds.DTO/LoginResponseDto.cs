namespace HiveMinds.DTO;

public class LoginResponseDto
{
    public int AccountId { get; set; }
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}