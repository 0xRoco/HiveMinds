using HiveMinds.Models;

namespace HiveMinds.ViewModels;

public class ThoughtViewModel
{
    public int Id { get; init; }
    public string Username { get; set; }
    public string Content { get; init; } = string.Empty;
    public List<ThoughtLike>? Likes { get; init; } = new();
    public List<ThoughtReplyViewModel>? Replies { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}