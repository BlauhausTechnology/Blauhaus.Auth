using System.Diagnostics;
using Blauhaus.Analytics.Console._Ioc;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Server.Azure.AdalProxy;
using Blauhaus.Auth.Server.Azure.Config;
using Blauhaus.Auth.Server.Azure.Service;
using Blauhaus.Auth.Server.Azure.User;
using Blauhaus.HttpClientService._Ioc;
using Blauhaus.Ioc.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Auth.Server.Azure._Ioc
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection RegisterAzureAuthenticationServer<TConfig, TUser>(this IServiceCollection services, TraceListener consoleTraceListener) 
            where TConfig : class, IAzureActiveDirectoryServerConfig where TUser : BaseAzureActiveDirectoryUser
        {
            services.RegisterConsoleLoggerClientService();
            RegisterCommon<TConfig, TUser>(services, consoleTraceListener);
            return services;
        }

        public static IServiceCollection RegisterAzureAuthenticationServer<TConfig>(this IServiceCollection services, TraceListener consoleTraceListener) 
            where TConfig : class, IAzureActiveDirectoryServerConfig 
        {
            services.RegisterConsoleLoggerService(consoleTraceListener);
            RegisterCommon<TConfig, DefaultAzureActiveDirectoryUser>(services, consoleTraceListener);
            return services;
        }

        private static void RegisterCommon<TConfig, TUser>(IServiceCollection services, TraceListener consoleTraceListener) 
            where TConfig : class, IAzureActiveDirectoryServerConfig
            where TUser : BaseAzureActiveDirectoryUser
        {
            services.RegisterServerHttpService(consoleTraceListener);
            services.AddScoped<IAzureAuthenticationServerService<TUser>, AzureAuthenticationServerService<TUser>>();
            services.AddTransient<IAzureActiveDirectoryUser, TUser>();
            services.AddTransient<TUser>();
            services.AddScoped<IAzureActiveDirectoryServerConfig, TConfig>();
            services.AddScoped<IAdalAuthenticationContextProxy, AdalAuthenticationContextProxy>();
        }
    }
}