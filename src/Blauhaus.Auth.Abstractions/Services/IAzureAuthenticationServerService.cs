using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Blauhaus.Auth.Abstractions.Services
{
    public interface IAzureAuthenticationServerService<TUser> where TUser : class, IAzureActiveDirectoryUser //todo remove TUser rather expose claims
    {
        Task SetCustomClaimAsync(string userObjectId, string propertyName, string value, CancellationToken token);
        Task SetCustomClaimsAsync(string userObjectId, Dictionary<string, string> claims, CancellationToken token);
        Task<TUser> GetUserAsync(string userObjectId, CancellationToken token);
        TUser ExtractUser(ClaimsPrincipal claimsPrincipal);
    }
}