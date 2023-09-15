using System.ComponentModel.DataAnnotations.Schema;

namespace HiveMinds.Models;

[Table("like")]
public class ThoughtLike
{
    public int Id { get; set; }
    public int ThoughtId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}