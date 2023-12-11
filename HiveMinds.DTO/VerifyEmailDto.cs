namespace HiveMinds.DTO;

public class VerifyEmailDto
{
    public string Username { get; set; } = string.Empty;
    public string VerificationCode { get; set; } = string.Empty;
}