namespace HiveMinds.Models;

public class ThoughtReport
{
    public int Id { get; set; }
    public int ThoughtId { get; set; }
    public int ReporterId { get; set; }
    public string Reason { get; set; } = null!;
    public ReportStatus Status { get; set; }
    public DateTime Date { get; set; }
    
    public enum ReportStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
    }
}