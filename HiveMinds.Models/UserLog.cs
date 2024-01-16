using System.ComponentModel.DataAnnotations;

namespace HiveMinds.Models;

public class UserLog
{
    [Range(1, int.MaxValue)]
    public int Id { get; set; }

    [Range(1, int.MaxValue)] public int AccountId { get; set; }
    public string Username { get; set; } = null!;
    public UserAction Action { get; set; } 
    public DateTime Date { get; set; }


    public enum UserAction
    {
        Login = 0,
        Logout = 1,
        Register = 2,
        CreateThought = 3,
        CreateReply = 4,
        LikeThought = 5,
        UnlikeThought = 6,
        ReportThought = 7,
        UpdateProfile= 8,
    }
}