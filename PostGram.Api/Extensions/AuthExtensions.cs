using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PostGram.Api.Middlewares;
using PostGram.BLL.Services;
using PostGram.Common.Configs;

namespace PostGram.Api.Extensions;

internal static class AuthExtensions
{
    public static void AddAuth(this IServiceCollection services, AuthConfig config)
    {
        //Authentication setup
        services.AddAuthentication(o => o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = config.Issuer,
                    ValidateAudience = true,
                    ValidAudience = config.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = TokenService.GetSymmetricSecurityKey(config.Key),
                    ClockSkew = TimeSpan.Zero
                };
            });

        //Authorization setup
        services.AddAuthorization(o => o.AddPolicy("ValidAccessToken", p =>
        {
            p.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
            p.RequireAuthenticatedUser();
        }));
    }

    public static void UseAuth(this WebApplication application)
    {
        application.UseAuthentication();
        application.UseAuthorization();
        application.UseTokenValidator();
    }
}