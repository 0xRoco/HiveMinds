namespace HiveMinds.DTO;

public class AccountDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public string LoyaltyStatement { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string VerificationRequest { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneNumberVerified { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
}