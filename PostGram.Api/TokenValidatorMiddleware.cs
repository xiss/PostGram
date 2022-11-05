﻿using PostGram.Api.Services;
using PostGram.Common;

namespace PostGram.Api
{
    public class TokenValidatorMiddleware
    {
        private readonly RequestDelegate _next;
        public TokenValidatorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            bool flag = true;
            string? sessionIdString = context.User.Claims.FirstOrDefault(c => c.Type == Constants.ClaimTypeSessionId)?.Value;
            if (Guid.TryParse(sessionIdString, out Guid sessionId))
            {
                var session = await userService.GetUserSessionById(sessionId);
                if (!session.IsActive)
                {
                    flag = false;
                    context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    //TODO Тут не нужно исключение выдавать или в лог писать?
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
}
