using HiveMinds.DTO;
using HiveMinds.Models;

namespace HiveMinds.ViewModels;

public class ThoughtViewModel
{
    public int Id { get; init; }
    public UserDto Author { get; set; } = new();
    public string Content { get; init; } = string.Empty;
    public List<ThoughtLike>? Likes { get; set; } = new();
    public List<ReplyViewModel>? Replies { get; set; } = new();
    public DateTime CreatedAt { get; init; }
}