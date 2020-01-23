using System.Collections.Generic;

namespace Blauhaus.Auth.Abstractions.Models
{
    public class UserAuthentication : IUserAuthentication
    {
        private UserAuthentication(
            UserAuthenticationState authenticationState, 
            AuthenticationMode mode, 
            string authenticatedUserId, 
            string authToken,
            string errorMessage = "")
        {
            AuthenticatedUserId = authenticatedUserId;
            AuthenticatedAccessToken = authToken;
            AuthenticationState = authenticationState;
            AuthenticationMode = mode;
            ErrorMessage = errorMessage;
        }

        public bool IsAuthenticated => AuthenticationState ==
                                       UserAuthenticationState.Authenticated &&
                                       !string.IsNullOrEmpty(AuthenticatedUserId) &&
                                       !string.IsNullOrEmpty(AuthenticatedAccessToken);

        public UserAuthenticationState AuthenticationState { get; }
        public AuthenticationMode AuthenticationMode { get; }
        public string AuthenticatedUserId { get; }
        public string AuthenticatedAccessToken { get; }
        public string ErrorMessage { get; }

        public Dictionary<string, string> AuthenticationProperties { get; } = new Dictionary<string, string>();

        public static IUserAuthentication CreateAuthenticated(string userId, string sessionToken, AuthenticationMode mode)
        {
            return new UserAuthentication(UserAuthenticationState.Authenticated, mode, userId, sessionToken);
        }
        
        public static IUserAuthentication CreateCancelled(AuthenticationMode mode)
        {
            return new UserAuthentication(UserAuthenticationState.Cancelled, mode, string.Empty, string.Empty);
        }

        public static IUserAuthentication CreateFailed(string errorMessage, AuthenticationMode mode)
        {
            return new UserAuthentication(UserAuthenticationState.Failed, mode, string.Empty, string.Empty, errorMessage);
        }
    }
}