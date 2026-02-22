namespace InccApi.RateLimitConfig;

public class PerUserLimitConfig
{
    public int TokenLimit { get; set; } = 12;
    public int TokensPerPeriod { get; set; } = 4;
    public double ReplenishmentPeriod { get; set; } = 1;
    public int QueueLimit { get; set; } = 0;
    public bool AutoReplenishment { get; set; } = true;
}
