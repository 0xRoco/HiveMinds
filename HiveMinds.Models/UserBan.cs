using System.ComponentModel.DataAnnotations;

namespace HiveMinds.Models;

public class UserBan
{
    [Range(1, int.MaxValue)]
    public int Id { get; set; }

    [Range(1, int.MaxValue)] public int AccountId { get; set; }

    [Range(1, int.MaxValue)]
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