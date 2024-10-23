using AspNetCoreRateLimit;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using PostGram.Api.Extensions;
using PostGram.Api.Middlewares;
using PostGram.Common.Configs;
using PostGram.DAL;
using SimpleInjector;

//Nlog setup
Logger? logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("Init main");

try
{

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    //Configuration
    IConfigurationSection authSection = builder.Configuration.GetSection(AuthConfig.SectionName);
    AuthConfig? authConfig = authSection.Get<AuthConfig>();

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    //Services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerConfigured();
    builder.Services.AddOptions();
    builder.Services.AddMemoryCache();
    builder.Services.AddInMemoryRateLimiting();
    builder.Services.AddDbContext<DataContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"),
            ob => ob.MigrationsAssembly(typeof(Program).Assembly.GetName().Name)));
    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    builder.Services.AddAuth(authConfig);
    builder.Services.AddSimpleInjectorConfigured(builder.Configuration, out Container container);

    WebApplication app = builder.Build();
    app.Services.UseAndVerifySimpleInjector(container);

    //Migration
    using (IServiceScope? serviceScope = ((IApplicationBuilder)app).ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
    {
        if (serviceScope != null)
        {
            DataContext context = serviceScope.ServiceProvider.GetRequiredService<PostGram.DAL.DataContext>();
            context.Database.Migrate();
        }
    }

    app.UseSwaggerWithUi();
    app.UseDeveloperExceptionPage();
    app.UseHttpsRedirection();
    app.UseAuth();
    app.UseClientRateLimiting();
    app.UseGlobalErrorHandlerMiddleWare();
    app.MapControllers();

    app.Run();
}
catch (Exception e)
{
    logger.Error(e, "Cannot start main");
    throw;
}
finally
{
    LogManager.Shutdown();
}