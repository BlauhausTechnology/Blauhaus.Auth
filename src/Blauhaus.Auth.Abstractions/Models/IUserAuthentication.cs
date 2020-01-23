using System.Collections.Generic;

namespace Blauhaus.Auth.Abstractions.Models
{
    public interface IUserAuthentication
    {
        bool IsAuthenticated { get; }
        UserAuthenticationState AuthenticationState { get; }
        SuccessfulAuthenticationMode AuthenticationMode { get; }
        string AuthenticatedUserId { get; }
        string AuthenticatedAccessToken { get; }
        string ErrorMessage { get; }
    }
}