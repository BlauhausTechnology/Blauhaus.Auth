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

        //public static IIocService RegisterAzureAuthenticationServer<TConfig>(this IIocService iocService) 
        //    where TConfig : class, IAzureActiveDirectoryServerConfig 
        //{
        //    iocService.RegisterConsoleLoggerClientService();
        //    iocService.RegisterClientHttpService();
        //    iocService.RegisterImplementation<IAzureAuthenticationServerService, AzureAuthenticationServerService>(IocLifetime.Transient);
        //    iocService.RegisterImplementation<IAuthenticatedUser, AuthenticatedUser>(IocLifetime.Transient);
        //    iocService.RegisterImplementation<IAzureActiveDirectoryServerConfig, TConfig>();
        //    iocService.RegisterImplementation<IAdalAuthenticationContextProxy, AdalAuthenticationContextProxy>(IocLifetime.Transient);

        //    return iocService;
        //}
    }
}