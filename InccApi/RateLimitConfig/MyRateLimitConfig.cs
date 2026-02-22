namespace InccApi.RateLimitConfig;

public class MyRateLimitConfig
{
    public GlobalLimitConfig Global { get; set; } = new();
    public PerUserLimitConfig PerUser { get; set; } = new();
}
