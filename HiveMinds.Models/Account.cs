using System.ComponentModel.DataAnnotations.Schema;
using HiveMinds.Common;

namespace HiveMinds.Models;

[Table("account")]
public class Account
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public AccountStatus Status { get; set; }
    public AccountRole Role { get; set; }
    public string EmailCode { get; set; } = string.Empty;
    public string PhoneNumberCode { get; set; } = string.Empty;
    public string PasswordResetToken { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public string LoyaltyStatement { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneNumberVerified { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
    public string LastLoginIp { get; set; } = string.Empty;
}