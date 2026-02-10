using System.Text.Json;

namespace InccApi.DTOs;

public class ErrorDetails
{
    public string TraceId { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public string? Detail { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
