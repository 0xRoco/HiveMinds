namespace HiveMinds.DTO;

public class ProfileDto
{
    public UserDto User { get; set; } = new();
    public List<ThoughtDto> Thoughts { get; set; } = new();
    public List<LikeDto> Likes { get; set; } = new();
}