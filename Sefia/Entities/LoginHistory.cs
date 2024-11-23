namespace Sefia.Entities;

public class LoginHistory
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public User? User { get; set; }
    public DateTime LoginTime { get; set; } = DateTime.Now;
    public required string IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool IsSuccess { get; set; }
    public string? FailureReason { get; set; }
}
