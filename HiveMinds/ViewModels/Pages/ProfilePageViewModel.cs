namespace HiveMinds.ViewModels.Pages;

public class ProfilePageViewModel : BasePageViewModel
{
    public UserViewModel Profile { get; set; } = new();
}