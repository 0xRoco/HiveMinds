using HiveMinds.Models;

namespace HiveMinds.ViewModels;

public class UserViewModel
{
    public string ProfilePicture { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public List<ThoughtViewModel>? Thoughts { get; set; } = new();
    
    public List<ThoughtLike>? Likes { get; set; } = new();
    public bool IsVerified { get; set; }
    public bool IsSuspended { get; set; }
    public bool IsDeactivated { get; set; }
    public DateTime Joined { get; set; }
    public DateTime LastSeen { get; set; }
}