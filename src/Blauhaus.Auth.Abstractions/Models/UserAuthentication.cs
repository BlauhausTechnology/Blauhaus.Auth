using System.Collections.Generic;

namespace Blauhaus.Auth.Abstractions.Models
{
    public class UserAuthentication : IUserAuthentication
    {
        private UserAuthentication(UserAuthenticationState authenticationState, string authenticatedUserId, string authToken)
        {
            AuthenticatedUserId = authenticatedUserId;
            AuthenticatedAccessToken = authToken;
            AuthenticationState = authenticationState;
        }

        public bool IsAuthenticated => AuthenticationState ==
                                       UserAuthenticationState.Authenticated &&
                                       !string.IsNullOrEmpty(AuthenticatedUserId) &&
                                       !string.IsNullOrEmpty(AuthenticatedAccessToken);

        public UserAuthenticationState AuthenticationState { get; }
        public string AuthenticatedUserId { get; }
        public string AuthenticatedAccessToken { get; }
        public Dictionary<string, string> AuthenticationProperties { get; } = new Dictionary<string, string>();

        public static IUserAuthentication CreateAuthenticated(string userId, string sessionToken)
        {
            return new UserAuthentication(UserAuthenticationState.Authenticated,  userId, sessionToken);
        }
        
        public static IUserAuthentication CreateUnauthenticated(UserAuthenticationState unauthenticatedState)
        {
            return new UserAuthentication(unauthenticatedState, string.Empty, string.Empty);
        }

        
        public static IUserAuthentication CreateCancelled()
        {
            return new UserAuthentication(UserAuthenticationState.Cancelled, string.Empty, string.Empty);
        }
    }
}