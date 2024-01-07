using HiveMinds.Common;

namespace HiveMinds.DTO;

public class VerificationDto
{
    public int Id { get; init; }
    public UserDto User { get; set; } = new();
    public string Reason { get; init; } = string.Empty;
    public VerificationStatus Status { get; set; }
    public DateTime CreatedAt { get; init; }
}