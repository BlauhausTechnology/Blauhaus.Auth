using System.Collections.Generic;

namespace Blauhaus.Auth.Abstractions.Models
{
    public interface IUserAuthentication
    {
        bool IsAuthenticated { get; }
        UserAuthenticationState AuthenticationState { get; }
        AuthenticationMode AuthenticationMode { get; }
        string AuthenticatedAccessToken { get; }
        string ErrorMessage { get; }
        string AuthenticatedUserId { get; } //todo change to TUser where TUser : class, IAzureActiveDirectoryUser
    }
}