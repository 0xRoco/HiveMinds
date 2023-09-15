namespace HiveMinds.Models;

public class UserBan
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AdminId { get; set; }
    public string Reason { get; set; } = null!;
    public BanStatus Status { get; set; }
    public DateTime Date { get; set; }
    public DateTime Expiration { get; set; }
    
    public enum BanStatus
    {
        Active = 0,
        Expired = 1,
        Revoked = 2,
    }
}