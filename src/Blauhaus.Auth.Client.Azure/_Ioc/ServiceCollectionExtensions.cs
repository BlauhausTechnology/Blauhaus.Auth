using Blauhaus.Analytics.Console._Ioc;
using Blauhaus.Auth.Abstractions._Ioc;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Client.Azure.Config;
using Blauhaus.Auth.Client.Azure.MsalProxy;
using Blauhaus.Auth.Client.Azure.Service;
using Blauhaus.Common.Time._Ioc;
using Blauhaus.Ioc.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Auth.Client.Azure._Ioc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureAuthenticationClient<TConfig>(this IServiceCollection services) 
            where TConfig : class, IAzureActiveDirectoryClientConfig
        {
            Register<TConfig>(services);
            services.RegisterAccessToken();
            return services;
        }

        private static void Register<TConfig>(IServiceCollection services) where TConfig : class, IAzureActiveDirectoryClientConfig
        {
            services.AddScoped<IAzureActiveDirectoryClientConfig, TConfig>();
            services.AddScoped<IAuthenticationClientService, AzureAuthenticationClientService>();
            services.AddScoped<IMsalClientProxy, MsalClientProxy>();
            services.RegisterConsoleLoggerClientService();
            services.RegisterTimeService();

        }
    }
}