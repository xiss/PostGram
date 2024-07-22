using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using PostGram.Api;
using PostGram.Api.Configs;
using PostGram.Api.Middlewares;
using PostGram.Common.Constants;

var builder = WebApplication.CreateBuilder(args);

//Configuration
var authSection = builder.Configuration.GetSection(AuthConfig.SectionName);
var authConfig = authSection.Get<AuthConfig>();

//Nlog setup
LogManager.Setup().LoadConfigurationFromAppSettings();
builder.Logging.ClearProviders();
builder.Host.UseNLog();

//Services
builder.Services.AddControllers();

//Swagger setup
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
    o.SwaggerDoc(Api.EndpointAuthorizationName, new OpenApiInfo { Title = Api.EndpointAuthorizationName });
    o.SwaggerDoc(Api.EndpointApiName, new OpenApiInfo { Title = Api.EndpointApiName });
});

builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.Configure<AuthConfig>(authSection);
builder.Services.Configure<AppConfig>(builder.Configuration.GetSection(AppConfig.SectionName));


//ClientRateLimit setup
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting")); ;
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

builder.Services.AddDbContext<PostGram.DAL.DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"),
        ob => ob.MigrationsAssembly(typeof(Program).Assembly.GetName().Name)));

//Authentication setup
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

//Authorization setup
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
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(Api.EndpointApiName + "/swagger.json", Api.EndpointApiName);
    options.SwaggerEndpoint(Api.EndpointAuthorizationName + "/swagger.json", Api.EndpointAuthorizationName);
});
app.UseDeveloperExceptionPage();
//}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseClientRateLimiting();
app.UseAuthorization();
app.UseTokenValidator();

app.UseGlobalErrorHandlerMiddleWare();
app.MapControllers();

app.Run();