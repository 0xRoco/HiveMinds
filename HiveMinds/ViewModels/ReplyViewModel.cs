namespace HiveMinds.ViewModels;

public class ReplyViewModel
{
    public int Id { get; set; }
    public int parentId { get; set; }
    public string Username { get; set; }
    public string Content { get; init; }
    public DateTime CreatedAt { get; set; }
}