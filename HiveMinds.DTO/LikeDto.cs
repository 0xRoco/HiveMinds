namespace HiveMinds.DTO;

public class LikeDto
{
    public int Id { get; init; }
    public int ThoughtId { get; set; }
    public UserDto? User { get; set; } = new();
    public DateTime CreatedAt { get; init; }
}