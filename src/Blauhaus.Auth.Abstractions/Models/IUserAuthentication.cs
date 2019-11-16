using System.Collections.Generic;

namespace Blauhaus.Auth.Abstractions.Models
{
    public interface IUserAuthentication
    {
        bool IsAuthenticated { get; }
        UserAuthenticationState AuthenticationState { get; }
        string AuthenticatedUserId { get; }
        string AuthenticatedAccessToken { get; }
        Dictionary<string, string> AuthenticationProperties { get; }
    }
}