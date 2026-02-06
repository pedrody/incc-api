using InccApi.DTOs;
using System.Text.Json;

namespace InccApi.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var isDevelopment = Environment.GetEnvironmentVariable
            ("ASPNETCORE_ENVIRONMENT") == "Development";

        var errorDetails = new ErrorDetails
        {
            StatusCode = context.Response.StatusCode,
            Message = isDevelopment ? ex.Message : "Ocorreu um erro interno no servidor",
            Detail = isDevelopment ? ex.StackTrace : null
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorDetails, options));
    }
}
