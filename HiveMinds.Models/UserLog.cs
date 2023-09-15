namespace HiveMinds.Models;

public class UserLog
{
    public int Id { get; set; } 
    public int UserId { get; set; }
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