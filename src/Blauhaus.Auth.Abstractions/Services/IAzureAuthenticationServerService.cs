using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Blauhaus.Auth.Abstractions.Services
{
    public interface IAzureAuthenticationServerService
    {
        Task SetCustomClaimsAsync(Guid userId, Dictionary<string, string> claims, CancellationToken token);
        Task<IAuthenticatedUser> GetUserFromAzureAsync(Guid userId, CancellationToken token);
        IAuthenticatedUser ExtractUserFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal);
    }
}