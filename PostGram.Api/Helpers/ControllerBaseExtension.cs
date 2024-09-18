using AspNetCoreRateLimit;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PostGram.BLL;
using PostGram.BLL.Interfaces.Services;
using PostGram.BLL.Services;
using PostGram.Common.Constants;
using PostGram.Common.Exceptions;
using SimpleInjector;

namespace PostGram.Api.Helpers;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSimpleInjectorConfigured(this IServiceCollection services)
    {
        var container = new Container();
        container.AddServices();
        container.Register<IUserService, UserService>(Lifestyle.Singleton);
        services.AddSimpleInjector(container, options =>
        {
            options.AddAspNetCore()
                .AddControllerActivation()
                .AddViewComponentActivation();
            options.AddLogging();
        });

        
#if DEBUG
        container.Verify();
#endif

        return services;
    }

    private static Container AddServices(this Container container)
    {
        container.Register<ITokenService, TokenService>(Lifestyle.Scoped);
        container.Register<IAttachmentService, AttachmentService>(Lifestyle.Scoped);
        container.Register<IPostService, PostService>(Lifestyle.Scoped);
        //container.Register<IRateLimitConfiguration, RateLimitConfiguration>(Lifestyle.Singleton);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MapperProfile());
        });
        container.RegisterInstance<MapperConfiguration>(config);
        container.Register<IMapper>(() => config.CreateMapper(container.GetInstance));

        return container;
    }

}