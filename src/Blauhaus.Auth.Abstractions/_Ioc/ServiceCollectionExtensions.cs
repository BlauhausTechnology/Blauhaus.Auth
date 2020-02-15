using Blauhaus.Auth.Abstractions.AccessToken;
using Blauhaus.Ioc.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Auth.Abstractions._Ioc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterAccessToken(this IServiceCollection services)
        {
            //for some reason services.AddSingleton<IAuthenticatedAccessToken, AuthenticatedAccessToken>(); does not result in a singleton in some cases
            //so this approach is safer
            services.AddSingleton<IAuthenticatedAccessToken>(new AuthenticatedAccessToken());
            return services;
        }
        
    }
}