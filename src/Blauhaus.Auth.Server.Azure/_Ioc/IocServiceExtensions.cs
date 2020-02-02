using Blauhaus.Analytics.Console._Ioc;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Server.Azure.AdalProxy;
using Blauhaus.Auth.Server.Azure.Config;
using Blauhaus.Auth.Server.Azure.Service;
using Blauhaus.Auth.Server.Azure.User;
using Blauhaus.HttpClientService._Ioc;
using Blauhaus.Ioc.Abstractions;

namespace Blauhaus.Auth.Server.Azure._Ioc
{
    public static class IocServiceExtensions
    {
        public static IIocService RegisterAzureAuthenticationServer<TConfig, TUser>(this IIocService iocService) 
            where TConfig : class, IAzureActiveDirectoryServerConfig 
            where TUser : BaseAzureActiveDirectoryUser
        {
            RegisterCommon<TConfig, TUser>(iocService);
            return iocService;
        }

        public static IIocService RegisterAzureAuthenticationServer<TConfig>(this IIocService iocService) 
            where TConfig : class, IAzureActiveDirectoryServerConfig 
        {
            RegisterCommon<TConfig, DefaultAzureActiveDirectoryUser>(iocService);
            return iocService;
        }

        private static void RegisterCommon<TConfig, TUser>(IIocService iocService) 
            where TConfig : class, IAzureActiveDirectoryServerConfig
            where TUser : BaseAzureActiveDirectoryUser
        {
            iocService.RegisterConsoleLoggerClientService();
            iocService.RegisterClientHttpService();
            iocService.RegisterImplementation<IAzureAuthenticationServerService<TUser>, AzureAuthenticationServerService<TUser>>(IocLifetime.Transient);
            iocService.RegisterImplementation<IAzureActiveDirectoryUser, TUser>(IocLifetime.Transient);
            iocService.RegisterType<TUser>(IocLifetime.Transient);
            iocService.RegisterImplementation<IAzureActiveDirectoryServerConfig, TConfig>();
            iocService.RegisterImplementation<IAdalAuthenticationContextProxy, AdalAuthenticationContextProxy>(IocLifetime.Transient);
        }
    }
}