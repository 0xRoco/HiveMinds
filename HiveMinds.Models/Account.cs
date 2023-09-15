using System.ComponentModel.DataAnnotations.Schema;

namespace HiveMinds.Models;

[Table("account")]
public class Account
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public string PhoneNumber { get; set; }
    public string EmailCode { get; set; }
    public string PhoneNumberCode { get; set; }
    public string LoginToken { get; set; }
    public string PasswordResetToken { get; set; }
    public string ProfilePictureUrl { get; set; }
    public string LoyaltyStatement { get; set; }
    public string Bio { get; set; }
    public string VerificationRequest { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneNumberVerified { get; set; }
    public bool IsVerified { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
    public string LastLoginIp { get; set; }
}