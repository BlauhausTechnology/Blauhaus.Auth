using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Auth.Server.Azure.User;

namespace Blauhaus.Auth.Server.Azure.Service
{
    public interface IAzureAuthenticationServerService<TUser> where TUser : class, IAzureActiveDirectoryUser
    {
        Task SetCustomClaimAsync(string userObjectId, string propertyName, string value, CancellationToken token);
        Task SetCustomClaimsAsync(string userObjectId, Dictionary<string, string> claims, CancellationToken token);
        Task<TUser> GetUserAsync(string userObjectId, CancellationToken token);
        TUser ExtractUser(ClaimsPrincipal claimsPrincipal);
    }
}