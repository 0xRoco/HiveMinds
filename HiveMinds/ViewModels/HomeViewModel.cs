namespace HiveMinds.ViewModels;

public class HomeViewModel
{
    public UserViewModel User { get; set; } = new();
    public List<ThoughtViewModel> Thoughts { get; set; } = new();
}