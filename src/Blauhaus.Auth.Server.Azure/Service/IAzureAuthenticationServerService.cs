using System.Security.Claims;
using System.Threading.Tasks;
using Blauhaus.Auth.Server.Azure.User;

namespace Blauhaus.Auth.Server.Azure.Service
{
    public interface IAzureAuthenticationServerService<TUser> where TUser : class, IAzureActiveDirectoryUser
    {
        Task SetCustomClaimAsync(string userObjectId, string propertyName, string value);
        Task<TUser> GetUserAsync(string userObjectId);
        TUser ExtractUser(ClaimsPrincipal claimsPrincipal);
    }
}