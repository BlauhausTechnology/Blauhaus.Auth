using Blauhaus.Auth.Abstractions.AccessToken;
using Blauhaus.Ioc.Abstractions;

namespace Blauhaus.Auth.Abstractions.Ioc
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