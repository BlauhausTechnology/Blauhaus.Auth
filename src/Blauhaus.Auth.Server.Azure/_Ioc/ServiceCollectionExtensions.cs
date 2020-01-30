using System.Diagnostics;
using Blauhaus.Analytics.Console._Ioc;
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


        //Auth may not be able to support IServiceCollection because it does not seem to allow services resolution to work...

        public static IServiceCollection RegisterAzureAuthenticationServer<TConfig, TUser>(this IServiceCollection services, TraceListener consoleTraceListener) 
            where TConfig : class, IAzureActiveDirectoryServerConfig where TUser : BaseAzureActiveDirectoryUser
        {
            RegisterCommon<TConfig, TUser>(services, consoleTraceListener);
            return services;
        }

        public static IServiceCollection RegisterAzureAuthenticationServer<TConfig>(this IServiceCollection services, TraceListener consoleTraceListener) 
            where TConfig : class, IAzureActiveDirectoryServerConfig 
        {
            RegisterCommon<TConfig, DefaultAzureActiveDirectoryUser>(services, consoleTraceListener);
            return services;
        }

        private static void RegisterCommon<TConfig, TUser>(IServiceCollection services, TraceListener consoleTraceListener) 
            where TConfig : class, IAzureActiveDirectoryServerConfig
            where TUser : BaseAzureActiveDirectoryUser
        {
            services.RegisterConsoleLoggerServerService(consoleTraceListener);
            services.RegisterServerHttpService(consoleTraceListener);
            services.AddScoped<IAzureAuthenticationServerService<TUser>, AzureAuthenticationServerService<TUser>>();
            services.AddTransient<IAzureActiveDirectoryUser, TUser>();
            services.AddTransient<TUser>();
            services.AddScoped<IAzureActiveDirectoryServerConfig, TConfig>();
            services.AddScoped<IAdalAuthenticationContextProxy, AdalAuthenticationContextProxy>();
        }
    }
}