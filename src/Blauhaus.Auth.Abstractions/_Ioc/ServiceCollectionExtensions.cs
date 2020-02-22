using Blauhaus.Auth.Abstractions.AccessToken;
using Blauhaus.Ioc.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Auth.Abstractions._Ioc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterAccessToken(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticatedAccessToken, AuthenticatedAccessToken>();
            return services;
        }
        
    }
}