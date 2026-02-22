using InccApi.Context;
using InccApi.Middlewares;
using InccApi.RateLimitConfig;
using InccApi.Repositories;
using InccApi.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.AzureAppServices;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Logging.AddAzureWebAppDiagnostics();
builder.Services.Configure<AzureFileLoggerOptions>(options =>
{
    options.FileName = "logs-";
    options.FileSizeLimit = 50 * 1024;
    options.RetainedFileCountLimit = 7;
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                               ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .SetPreflightMaxAge(TimeSpan.FromHours(1));
    });
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(connectionString));

var rateLimitConfig = builder.Configuration.GetSection("MyRateLimit")
                                           .Get<MyRateLimitConfig>()
                                           ?? new MyRateLimitConfig();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: "global",
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = rateLimitConfig.Global.PermitLimit,
                SegmentsPerWindow = rateLimitConfig.Global.SegmentsPerWindow,
                Window = TimeSpan.FromSeconds(rateLimitConfig.Global.Window),
                QueueLimit = rateLimitConfig.Global.QueueLimit,
            })
        );

    options.AddPolicy("per-user", httpContext =>
    {
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetTokenBucketLimiter(
            partitionKey: clientIp, 
            _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = rateLimitConfig.PerUser.TokenLimit,
                TokensPerPeriod = rateLimitConfig.PerUser.TokensPerPeriod,
                ReplenishmentPeriod = TimeSpan.FromSeconds(rateLimitConfig.PerUser.ReplenishmentPeriod),
                QueueLimit = rateLimitConfig.PerUser.QueueLimit,
                AutoReplenishment = rateLimitConfig.PerUser.AutoReplenishment,
        });
    });

    options.OnRejected = async (context, token) =>
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogWarning("Request from {ClientIp} was rate limited.", context.HttpContext.Connection.RemoteIpAddress);

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Too many requests. Please try again later.",
            message = "You have exceeded the allowed request limit. Please wait before making more requests."
        }, cancellationToken: token);
    };
});

builder.Services.AddScoped<IInccRepository, InccRepository>();
builder.Services.AddScoped<IInccService, InccService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<ExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "incc api"));
}

app.UseHttpsRedirection();

app.UseCors();

app.UseForwardedHeaders();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();
