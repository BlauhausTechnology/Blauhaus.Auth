using Blauhaus.Auth.Abstractions.AccessToken;
using Blauhaus.Ioc.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Auth.Abstractions._Ioc
{
    public static class ServiceCollectionExtensions
    {
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