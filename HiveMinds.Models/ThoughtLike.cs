using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiveMinds.Models;

[Table("like")]
public class ThoughtLike
{
    public int Id { get; set; }
    public int ThoughtId { get; set; }
    [Range(1, int.MaxValue)] public int AccountId { get; set; }
    public DateTime CreatedAt { get; set; }
}