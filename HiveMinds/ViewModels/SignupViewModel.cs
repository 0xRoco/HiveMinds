using System.ComponentModel.DataAnnotations;

namespace HiveMinds.ViewModels;

public class SignupViewModel
{
    [Required]
    public string Username { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }
    [Required]
    public string PartyLoyaltyStatement { get; set; }
    [Required]
    public string Bio { get; set; }
    public string PhoneNumber { get; set; }
}