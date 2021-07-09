using System.Diagnostics;
using Blauhaus.Analytics.Console.Ioc;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Common.Ioc;
using Blauhaus.Auth.Server.Azure.AdalProxy;
using Blauhaus.Auth.Server.Azure.Config;
using Blauhaus.Auth.Server.Azure.Service;
using Blauhaus.HttpClientService.Ioc;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Auth.Server.Azure.Ioc
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddAzureAuthenticationServer<TConfig>(this IServiceCollection services, TraceListener consoleTraceListener) 
            where TConfig : class, IAzureActiveDirectoryServerConfig 
        {
            services.RegisterConsoleLoggerService(consoleTraceListener);
            services.AddServerHttpService(consoleTraceListener);
            services.AddScoped<IAzureAuthenticationServerService, AzureAuthenticationServerService>();
            services.AddScoped<IAzureActiveDirectoryServerConfig, TConfig>();
            services.AddScoped<IAdalAuthenticationContextProxy, AdalAuthenticationContextProxy>();

            return services.AddAzureUserFactory();
        }

    }
}