using HiveMinds.Models;

namespace HiveMinds.ViewModels.Pages.Admin;

public class AdminVerificationsViewModel
{
   public List<VerificationRequest> Requests { get; set; } = new();
}