using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PostGram.BLL.Interfaces.Providers;
using PostGram.Common.Constants;
using PostGram.Common.Exceptions;

namespace PostGram.BLL.Providers;

public class HttpContextClaimsProvider : IClaimsProvider
{
    private readonly ClaimsPrincipal? _claimsPrincipal;

    public HttpContextClaimsProvider(IHttpContextAccessor httpContext)
    {
        _claimsPrincipal = httpContext.HttpContext?.User;
    }

    public Guid GetCurrentUserId()
    {
        string? userIdStr = _claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimNames.UserId)?.Value;
        if (Guid.TryParse(userIdStr, out Guid userId))
            return userId;

        throw new AuthorizationPostGramException("You are not authorized");
    }

    public Guid GetCurrentSessionId()
    {
        string? sessionIdStr = _claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimNames.SessionId)?.Value;
        if (Guid.TryParse(sessionIdStr, out Guid userId))
            return userId;

        throw new AuthorizationPostGramException("You are not authorized");
    }
}