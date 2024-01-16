using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiveMinds.Models;

[Table("reply")]
public class ThoughtReply
{
    public int Id { get; set; }
    public int ThoughtId { get; set; }
    [Range(1, int.MaxValue)] public int AccountId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
}