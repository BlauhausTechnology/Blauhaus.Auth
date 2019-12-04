using Blauhaus.Auth.Server.Azure.AdalProxy;
using Blauhaus.Auth.Server.Azure.Config;
using Blauhaus.Auth.Server.Azure.Service;
using Blauhaus.Auth.Server.Azure.User;
using Blauhaus.HttpClientService._Ioc;
using Blauhaus.Ioc.Abstractions;

namespace Blauhaus.Auth.Server.Azure._Ioc
{
    public static class IocRegistration
    {
        public static IIocService RegisterAzureAuthenticationServer<TConfig, TUser>(this IIocService iocService) 
            where TConfig : class, IAzureActiveDirectoryServerConfig 
            where TUser : class, IAzureActiveDirectoryUser
        {
            iocService.RegisterImplementation<IAzureActiveDirectoryUser, TUser>(IocLifetime.Transient);
            iocService.RegisterImplementation<IAzureAuthenticationServerService<TUser>, AzureAuthenticationServerService<TUser>>(IocLifetime.Transient);
            RegisterCommon<TConfig>(iocService);
            return iocService;
        }

        public static IIocService RegisterAzureAuthenticationServer<TConfig>(this IIocService iocService) 
            where TConfig : class, IAzureActiveDirectoryServerConfig 
        {
            iocService.RegisterImplementation<IAzureActiveDirectoryUser, DefaultAzureActiveDirectoryUser>(IocLifetime.Transient);
            iocService.RegisterImplementation<IAzureAuthenticationServerService<IAzureActiveDirectoryUser>, AzureAuthenticationServerService<IAzureActiveDirectoryUser>>(IocLifetime.Transient);
            RegisterCommon<TConfig>(iocService);
            return iocService;
        }

        private static void RegisterCommon<TConfig>(IIocService iocService) 
            where TConfig : class, IAzureActiveDirectoryServerConfig
        {
            iocService.RegisterHttpService();
            iocService.RegisterImplementation<IAzureActiveDirectoryServerConfig, TConfig>();
            iocService.RegisterImplementation<IAdalAuthenticationContextProxy, AdalAuthenticationContextProxy>(IocLifetime.Transient);

        }
    }
}