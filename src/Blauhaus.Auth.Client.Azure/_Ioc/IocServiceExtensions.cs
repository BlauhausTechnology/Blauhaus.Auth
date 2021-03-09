using Blauhaus.Analytics.Console.Ioc;
using Blauhaus.Auth.Abstractions._Ioc;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Client.Azure.Config;
using Blauhaus.Auth.Client.Azure.MsalProxy;
using Blauhaus.Auth.Client.Azure.Service;
using Blauhaus.Ioc.Abstractions;
using Blauhaus.Time._Ioc;

namespace Blauhaus.Auth.Client.Azure._Ioc
{
    public static class IocServiceExtensions
    {
        public static IIocService RegisterAzureAuthenticationClient<TConfig>(this IIocService iocService) 
            where TConfig : class, IAzureActiveDirectoryClientConfig
        {
            Register<TConfig>(iocService);
            iocService.RegisterAccessToken();
            return iocService;
        }

        private static void Register<TConfig>(IIocService iocService) where TConfig : class, IAzureActiveDirectoryClientConfig
        {
            iocService.RegisterImplementation<IAzureActiveDirectoryClientConfig, TConfig>(IocLifetime.Singleton);
            iocService.RegisterImplementation<IAuthenticationClientService, AzureAuthenticationClientService>(IocLifetime.Singleton);
            iocService.RegisterImplementation<IMsalClientProxy, MsalClientProxy>(IocLifetime.Transient);
            iocService.RegisterConsoleLoggerClientService();
            iocService.AddTimeService();

        }
    }
}