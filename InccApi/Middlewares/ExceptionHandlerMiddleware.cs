using InccApi.DTOs;
using System.Text.Json;

namespace InccApi.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(
        RequestDelegate next, 
        ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var traceId = Guid.NewGuid().ToString();
            _logger.LogError(ex, "Ocorreu um erro não tratado. TraceId: {traceId}", traceId);
            await HandleExceptionAsync(context, ex, traceId);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex, string traceId)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.Headers.Append("X-Trace-Id", traceId);

        var isDevelopment = Environment.GetEnvironmentVariable
            ("ASPNETCORE_ENVIRONMENT") == "Development";

        var errorDetails = new ErrorDetails
        {
            TraceId = traceId,
            StatusCode = context.Response.StatusCode,
            Message = isDevelopment ? ex.Message : "Ocorreu um erro interno no servidor",
            Detail = isDevelopment ? ex.StackTrace : null
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorDetails, options));
    }
}
