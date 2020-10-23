using System.Security.Claims;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Responses;
using CSharpFunctionalExtensions;

namespace Blauhaus.Auth.Abstractions.Services
{
    public interface IAuthenticatedUserFactory
    {
        Response<IAuthenticatedUser> ExtractFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal);
        Response<IAuthenticatedUser> ExtractFromJwtToken(string jwtToken);
    }
}