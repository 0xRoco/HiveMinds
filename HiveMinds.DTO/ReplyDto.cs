namespace HiveMinds.DTO;

public class ReplyDto
{
    public int Id { get; init; }
    public int ThoughtId { get; set; }
    public UserDto User { get; set; } = new();
    public string Content { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}