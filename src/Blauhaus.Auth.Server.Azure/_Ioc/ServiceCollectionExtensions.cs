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


        //Auth does not support IServiceCollection because it does not seem to allow IocService resolution to work...



        //public static IServiceCollection RegisterAzureAuthenticationServer<TConfig, TUser>(this IServiceCollection services) 
        //    where TConfig : class, IAzureActiveDirectoryServerConfig 
        //    where TUser : class, IAzureActiveDirectoryUser
        //{
        //    services.AddTransient<IAzureActiveDirectoryUser, TUser>();
        //    services.AddScoped<IAzureAuthenticationServerService<TUser>, AzureAuthenticationServerService<TUser>>();
        //    RegisterCommon<TConfig>(services);
        //    return services;
        //}

        //public static IServiceCollection RegisterAzureAuthenticationServer<TConfig>(this IServiceCollection services) 
        //    where TConfig : class, IAzureActiveDirectoryServerConfig 
        //{
        //    services.AddTransient<IAzureActiveDirectoryUser, DefaultAzureActiveDirectoryUser>();
        //    services.AddScoped<IAzureAuthenticationServerService<IAzureActiveDirectoryUser>, AzureAuthenticationServerService<IAzureActiveDirectoryUser>>();
        //    RegisterCommon<TConfig>(services);
        //    return services;
        //}

        //private static void RegisterCommon<TConfig>(IServiceCollection iocService) 
        //    where TConfig : class, IAzureActiveDirectoryServerConfig
        //{
        //    iocService.RegisterHttpService();
        //    iocService.AddScoped<IAzureActiveDirectoryServerConfig, TConfig>();
        //    iocService.AddTransient<IAdalAuthenticationContextProxy, AdalAuthenticationContextProxy>();

        //}
    }
}