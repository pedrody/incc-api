namespace InccApi.RateLimitConfig;

public class GlobalLimitConfig
{
    public int PermitLimit { get; set; } = 100;
    public int SegmentsPerWindow { get; set; } = 10;
    public int Window { get; set; } = 1;
    public int QueueLimit { get; set; } = 0;
}

