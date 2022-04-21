using Blauhaus.Auth.Abstractions.User;

namespace Blauhaus.Auth.Abstractions.Models
{
    public interface IUserAuthentication
    {
        bool IsAuthenticated { get; }
        UserAuthenticationState AuthenticationState { get; }
        AuthenticationMode AuthenticationMode { get; }
        string? AuthenticatedAccessToken { get; }
        string? AuthenticatedIdToken { get; }
        string? ErrorMessage { get; }
        IAuthenticatedUser? User { get; } 
    }
}