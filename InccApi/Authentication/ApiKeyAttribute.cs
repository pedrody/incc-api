using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InccApi.Authentication;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAttribute : Attribute, IAuthorizationFilter
{
    private const string ApiKeyHeaderName = "X-Api-Key";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!IsApiKeyValid(context.HttpContext))
        {
            context.Result = new UnauthorizedResult();
        }
    }

    private bool IsApiKeyValid(HttpContext context)
    {
        string? apiKey = context.Request.Headers[ApiKeyHeaderName];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return false;
        }

        string actualApiKey = context.RequestServices
            .GetRequiredService<IConfiguration>()
            .GetValue<string>("Authentication:ApiKey")!;

        return apiKey == actualApiKey;
    }
}
