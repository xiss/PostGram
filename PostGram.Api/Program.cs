using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PostGram.Api;
using PostGram.Api.Configs;
using PostGram.Api.Services;
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    //Configuration
    var authSection = builder.Configuration.GetSection(AuthConfig.SectionName);
    var appSection = builder.Configuration.GetSection(AppConfig.SectionName);
    var authConfig = authSection.Get<AuthConfig>();

    //TODO 3 Перенести настройки Nlog в appsettings
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    //Services
    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(o =>
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
    });

    builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IAttachmentService, AttachmentService>();
    builder.Services.AddScoped<ICommentService, CommentService>();
    builder.Services.AddScoped<IPostService, PostService>();

    builder.Services.Configure<AuthConfig>(authSection);
    builder.Services.Configure<AppConfig>(appSection);

    builder.Services.AddDbContext<PostGram.DAL.DataContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql")));

    //Authentication
    builder.Services.AddAuthentication(o => o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o =>
        {
            o.RequireHttpsMetadata = false;
            o.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = authConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = authConfig.Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = authConfig.GetSymmetricSecurityKey(),
                ClockSkew = TimeSpan.Zero
            };
        });

    //Authorization
    builder.Services.AddAuthorization(o => o.AddPolicy("ValidAccessToken", p =>
    {
        p.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        p.RequireAuthenticatedUser();
    }));

    var app = builder.Build();

    //Migration
    using (var serviceScope = ((IApplicationBuilder)app).ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
    {
        if (serviceScope != null)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<PostGram.DAL.DataContext>();
            context.Database.Migrate();
        }
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseTokenValidator();

    app.MapControllers();

    app.Run();
}
catch (Exception e)
{
    logger.Error(e, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}

