using System.ComponentModel.DataAnnotations.Schema;
using HiveMinds.Common;

namespace HiveMinds.Models;

[Table("verification")]
public class VerificationRequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Reason { get; set; } = null!;
    public VerificationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}