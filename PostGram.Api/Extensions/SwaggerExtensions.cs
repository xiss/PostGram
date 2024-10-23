using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using PostGram.Common.Constants;

namespace PostGram.Api.Extensions;

internal static class SwaggerExtensions
{
    public static void AddSwaggerConfigured(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(o =>
        {
            o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
            {
                Description = "Введите имя пользователя",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });
            o.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        },
                        Scheme = "oauth2",
                        Name = JwtBearerDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
            o.SwaggerDoc(EndpointApiNames.Authorization, new OpenApiInfo { Title = EndpointApiNames.Authorization });
            o.SwaggerDoc(EndpointApiNames.Api, new OpenApiInfo { Title = EndpointApiNames.Api });
        });
    }

    public static void UseSwaggerWithUi(this WebApplication application)
    {
        application.UseSwagger();
        application.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(EndpointApiNames.Api + "/swagger.json",EndpointApiNames.Api);
            options.SwaggerEndpoint(EndpointApiNames.Authorization + "/swagger.json", EndpointApiNames.Authorization);
        });
    }
}