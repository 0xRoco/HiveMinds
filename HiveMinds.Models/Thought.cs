using System.ComponentModel.DataAnnotations.Schema;

namespace HiveMinds.Models;

[Table("thought")]
public class Thought
{
    public int Id { get; set; }
    public int? ParentThoughtId { get; set; } // -1 or 0 means it's not a reply
    public int UserId { get; set; }
    public string Content { get; set; } = null!;
    public int Likes { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool Flagged { get; set; }
}