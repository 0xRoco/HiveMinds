using HiveMinds.Models;

namespace HiveMinds.ViewModels;

public class ThoughtViewModel
{
    public int Id { get; init; }
    public string Username { get; set; }
    public string Content { get; init; } = string.Empty;
    public List<ThoughtLike>? Likes { get; set; } = new();
    public List<ThoughtReplyViewModel>? Replies { get; set; } = new();
    public DateTime CreatedAt { get; init; }
}