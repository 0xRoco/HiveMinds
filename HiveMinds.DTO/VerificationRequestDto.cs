namespace HiveMinds.DTO;

public class VerificationRequestDto
{
    public int AccountId { get; set; }
    public string Reason { get; set; } = null!;
}