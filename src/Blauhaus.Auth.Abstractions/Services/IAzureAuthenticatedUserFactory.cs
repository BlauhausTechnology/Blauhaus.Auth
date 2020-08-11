using System.Security.Claims;
using Blauhaus.Auth.Abstractions.User;
using CSharpFunctionalExtensions;

namespace Blauhaus.Auth.Abstractions.Services
{
    public interface IAzureAuthenticatedUserFactory
    {
        Result<IAuthenticatedUser> Create(ClaimsPrincipal claimsPrincipal);
    }
}