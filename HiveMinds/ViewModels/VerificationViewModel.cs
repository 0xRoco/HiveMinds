using HiveMinds.DTO;

namespace HiveMinds.ViewModels;

public class VerificationViewModel
{
    public UserViewModel User { get; set; } = new();
    public VerificationDto? VerificationRequest { get; set; } = new();
}