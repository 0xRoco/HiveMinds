namespace HiveMinds.ViewModels;

public class EditProfileViewModel
{
    public IFormFile ProfilePicture { get; set; } = null!;
    public string Bio { get; set; } = "";
    public string PartyLoyaltyStatement { get; set; } = "";
}