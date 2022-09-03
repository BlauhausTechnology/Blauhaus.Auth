using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Common.UserFactory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Blauhaus.Auth.Common.Ioc
{
    public static class ServiceCollectionExtensions
    {
        
        public static IServiceCollection AddCommonAuthServices(this IServiceCollection services) 
        {
            services.TryAddTransient<IAuthenticatedUserFactory, AuthenticatedUserFactory>();
            return services;
        }
         

    }
}