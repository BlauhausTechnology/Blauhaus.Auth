using System.Diagnostics;
using Blauhaus.Analytics.Console._Ioc;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Auth.Server.Azure.AdalProxy;
using Blauhaus.Auth.Server.Azure.Config;
using Blauhaus.Auth.Server.Azure.Service;
using Blauhaus.HttpClientService._Ioc;
using Blauhaus.Ioc.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Blauhaus.Auth.Server.Azure._Ioc
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddAzureAuthenticationServer<TConfig>(this IServiceCollection services, TraceListener consoleTraceListener) 
            where TConfig : class, IAzureActiveDirectoryServerConfig 
        {
            services.RegisterConsoleLoggerService(consoleTraceListener);
            services.RegisterServerHttpService(consoleTraceListener);
            services.AddScoped<IAzureAuthenticationServerService, AzureAuthenticationServerService>();
            services.AddTransient<IAuthenticatedUser, AuthenticatedUser>();
            services.AddScoped<IAzureActiveDirectoryServerConfig, TConfig>();
            services.AddScoped<IAdalAuthenticationContextProxy, AdalAuthenticationContextProxy>();

            return services.AddAzureUserFactory();
        }

        public static IServiceCollection AddAzureUserFactory(this IServiceCollection services) 
        {
            services.TryAddScoped<IAuthenticatedUserFactory, AuthenticatedUserFactory>();
            services.TryAddTransient<IAuthenticatedUser, AuthenticatedUser>();
            return services;
        }
         
    }
}