namespace SecurityCenterAI.Domain.Entities;

public sealed class SecurityAnalysis
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public int Score { get; set; }
    public DateTime AnalyzedAtUtc { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
}
