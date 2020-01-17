using Blauhaus.Auth.Abstractions.ClientAuthenticationHandlers;
using Blauhaus.Ioc.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Auth.Abstractions._Ioc
{
    public static class IocRegistration
    {
        public static IIocService RegisterAccessToken(this IIocService iocService)
        {
            iocService.RegisterImplementation<IAuthenticatedAccessToken, AuthenticatedAccessToken>(IocLifetime.Singleton);
            return iocService;
        }
        
        public static IIocService RegisterAccessToken<TAccessToken>(this IIocService iocService) where TAccessToken : AuthenticatedAccessToken
        {
            iocService.RegisterImplementation<IAuthenticatedAccessToken, TAccessToken>(IocLifetime.Singleton);
            return iocService;
        }

        public static IServiceCollection RegisterAccessToken(this IServiceCollection services)
        {
            services.AddSingleton<IAuthenticatedAccessToken, AuthenticatedAccessToken>();
            return services;
        }
        
        public static IServiceCollection RegisterAccessToken<TAccessToken>(this IServiceCollection services) where TAccessToken : AuthenticatedAccessToken
        {
            services.AddSingleton<IAuthenticatedAccessToken, TAccessToken>();
            return services;
        }
    }
}