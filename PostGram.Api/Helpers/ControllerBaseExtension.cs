using Microsoft.AspNetCore.Mvc;
using PostGram.Common.Constants;
using PostGram.Common.Exceptions;

namespace PostGram.Api.Helpers
{
    public static class ControllerBaseExtension
    {
        public static Guid GetCurrentUserId(this ControllerBase controller)
        {
            string? userIdStr = controller.User.Claims.FirstOrDefault(c => c.Type == ClaimNames.UserId)?.Value;
            if (Guid.TryParse(userIdStr, out var userId))
                return userId;

            throw new AuthorizationPostGramException("You are not authorized");
        }
    }
}