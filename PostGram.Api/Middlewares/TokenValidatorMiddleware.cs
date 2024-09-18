using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Constants;

namespace PostGram.Api.Middlewares;

public class TokenValidatorMiddleware
{
    private readonly RequestDelegate _next;

    public TokenValidatorMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITokenService authService)
    {
        bool flag = true;
        string? sessionIdString = context.User.Claims.FirstOrDefault(c => c.Type == ClaimNames.SessionId)?.Value;
        if (Guid.TryParse(sessionIdString, out Guid sessionId))
        {
            var session = await authService.GetUserSessionById(sessionId);
            if (!session.IsActive)
            {
                flag = false;
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
        }

        if (flag)
            await _next(context);
    }
}

public static class TokenValidatorMiddlewareExctention
{
    public static IApplicationBuilder UseTokenValidator(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenValidatorMiddleware>();
    }
}