using Blauhaus.Auth.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Blauhaus.Auth.Common.Ioc
{
    public static class ServiceCollectionExtensions
    {
        
        public static IServiceCollection AddAzureUserFactory(this IServiceCollection services) 
        {
            services.TryAddScoped<IAuthenticatedUserFactory, AuthenticatedUserFactory>();
            return services;
        }

    }
}