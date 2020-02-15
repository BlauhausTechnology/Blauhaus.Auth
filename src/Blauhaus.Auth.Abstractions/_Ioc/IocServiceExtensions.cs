using Blauhaus.Auth.Abstractions.AccessToken;
using Blauhaus.Ioc.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Auth.Abstractions._Ioc
{
    public static class IocServiceExtensions
    {
        public static IIocService RegisterAccessToken(this IIocService iocService)
        {
            iocService.RegisterImplementation<IAuthenticatedAccessToken, AuthenticatedAccessToken>(IocLifetime.Singleton);
            return iocService;
        }
        

    }
}