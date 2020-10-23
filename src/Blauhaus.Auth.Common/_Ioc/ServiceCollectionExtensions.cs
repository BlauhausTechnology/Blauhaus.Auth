using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Abstractions.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Blauhaus.Auth.Common._Ioc
{
    public static class ServiceCollectionExtensions
    {
        
        public static IServiceCollection AddAzureUserFactory(this IServiceCollection services) 
        {
            services.TryAddScoped<IAuthenticatedUserFactory, AuthenticatedUserFactory>();
            services.TryAddTransient<IAuthenticatedUser, AuthenticatedUser>();
            return services;
        }

    }
}