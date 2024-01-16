using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HiveMinds.Common;

namespace HiveMinds.Models;

[Table("verification")]
public class VerificationRequest
{
    [Range(1, int.MaxValue)]
    public int Id { get; set; }

    [Range(1, int.MaxValue)] public int AccountId { get; set; }
    public string Reason { get; set; } = null!;
    public VerificationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}