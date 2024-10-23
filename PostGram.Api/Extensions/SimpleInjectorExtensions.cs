using System.Reflection;
using AspNetCoreRateLimit;
using AutoMapper;
using PostGram.BLL;
using PostGram.BLL.Interfaces.Providers;
using PostGram.BLL.Interfaces.Services;
using PostGram.BLL.Providers;
using PostGram.BLL.Services;
using PostGram.Common.Configs;
using PostGram.Common.Exceptions;
using PostGram.Common.Interfaces.Base.Commands;
using PostGram.Common.Interfaces.Base.Queries;
using SimpleInjector;

namespace PostGram.Api.Extensions;

internal static class SimpleInjectorExtensions
{
    public static void AddSimpleInjectorConfigured(this IServiceCollection services, IConfiguration configuration, out Container container)
    {
        container = new Container();

        services.AddSimpleInjector(container, options =>
            {
                options.AddAspNetCore()
                    .AddControllerActivation();
                options.AddLogging();
            })
            .BuildServiceProvider(validateScopes: true);
        container.RegisterComponents(configuration);
    }

    public static void UseAndVerifySimpleInjector(this IServiceProvider serviceProvider, Container container)
    {
        serviceProvider.UseSimpleInjector(container);
        container.Verify();
    }

    private static void RegisterComponents(this Container container, IConfiguration configuration)
    {
        container.RegisterAutoMapper();
        container.RegisterServices();
        container.RegisterCommandHandlers();
        container.RegisterQueryHandlers();
        container.RegisterProviders();
        container.RegisterConfigs(configuration);
    }

    private static void RegisterAutoMapper(this Container container)
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MapperProfile());
        });
        container.RegisterInstance(config);
        container.Register<IMapper>(() => config.CreateMapper(container.GetInstance));
    }

    private static void RegisterQueryHandlers(this Container container)
    {
        Assembly assembly = typeof(IClaimsProvider).Assembly;
        container.Register(typeof(IQueryHandler<,>), assembly, Lifestyle.Scoped);
    }

    private static void RegisterCommandHandlers(this Container container)
    {
        Assembly assembly = typeof(IClaimsProvider).Assembly;
        container.Register(typeof(ICommandHandler<>), assembly, Lifestyle.Scoped);
    }

    private static void RegisterServices(this Container container)
    {
        container.Register<IUserService, UserService>(Lifestyle.Scoped);
        container.Register<ITokenService, TokenService>(Lifestyle.Scoped);
        container.Register<IAttachmentService, AttachmentService>(Lifestyle.Scoped);
        container.Register<IPostService, PostService>(Lifestyle.Scoped);
        container.Register<ICommentService, CommentService>(Lifestyle.Scoped);
        container.Register<ISubscriptionsService, SubscriptionsService>(Lifestyle.Scoped);
    }

    private static void RegisterProviders(this Container container)
    {
        container.Register<IClaimsProvider, HttpContextClaimsProvider>(Lifestyle.Singleton);
        container.RegisterInstance(TimeProvider.System);
    }

    private static void RegisterConfigs(this Container container, IConfiguration configuration)
    {
        IConfigurationSection authSection = configuration.GetSection(AuthConfig.SectionName);
        IConfigurationSection appSection = configuration.GetSection(AppConfig.SectionName);
        IConfigurationSection clientRateLimitingSection = configuration.GetSection("ClientRateLimiting");

        AuthConfig authConfig = authSection.Get<AuthConfig>() ?? throw new NotFoundPostGramException("AuthConfig not loaded");
        AppConfig appConfig = appSection.Get<AppConfig>() ?? throw new NotFoundPostGramException("AppConfig not loaded");
        ClientRateLimitOptions clientRateLimitingConfig = clientRateLimitingSection.Get<ClientRateLimitOptions>()
            ?? throw new NotFoundPostGramException("ClientRateLimiting not loaded");

        container.RegisterInstance(authConfig);
        container.RegisterInstance(appConfig);
        container.RegisterInstance(clientRateLimitingConfig);
    }
}