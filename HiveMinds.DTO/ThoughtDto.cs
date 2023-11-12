namespace HiveMinds.DTO;

public class ThoughtDto
{
    public int Id { get; init; }
    public UserDto User { get; set; } = new();
    public string Content { get; init; } = string.Empty;
    public IEnumerable<ReplyDto> Replies { get; set; } 
    public IEnumerable<LikeDto> Likes { get; set; }
    public DateTime CreatedAt { get; init; }
}