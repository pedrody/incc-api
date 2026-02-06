using System.Text.Json;

namespace InccApi.DTOs;

public class ErrorDetails
{
    public ErrorDetails()
    {
        TraceId = Guid.NewGuid().ToString();
    }

    public string TraceId { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public string? Detail { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
