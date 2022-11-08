using PostGram.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using PostGram.Common;

namespace PostGram.Api
{
    public static class ControllerBaseExtension
    {
        // TODO Вот так нормально делать?
        public static Guid GetCurrentUserId(this ControllerBase controller)
        {
            string? userIdStr = controller.User.Claims.FirstOrDefault(c => c.Type == Constants.ClaimTypeUserId)?.Value;
            if (Guid.TryParse(userIdStr, out var userId))
                return userId;

            throw new AuthorizationPostGramException("You are not authorized");
        }
    }
}
