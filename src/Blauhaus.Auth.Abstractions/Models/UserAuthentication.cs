using System.Collections.Generic;

namespace Blauhaus.Auth.Abstractions.Models
{
    public class UserAuthentication : IUserAuthentication
    {
        private UserAuthentication(
            UserAuthenticationState authenticationState, 
            SuccessfulAuthenticationMode mode, 
            string authenticatedUserId, 
            string authToken, 
            long authenticationDurationMs = 0)
        {
            AuthenticatedUserId = authenticatedUserId;
            AuthenticatedAccessToken = authToken;
            AuthenticationDurationMs = authenticationDurationMs;
            AuthenticationState = authenticationState;
            AuthenticationMode = mode;
        }

        public bool IsAuthenticated => AuthenticationState ==
                                       UserAuthenticationState.Authenticated &&
                                       !string.IsNullOrEmpty(AuthenticatedUserId) &&
                                       !string.IsNullOrEmpty(AuthenticatedAccessToken);

        public UserAuthenticationState AuthenticationState { get; }
        public SuccessfulAuthenticationMode AuthenticationMode { get; }
        public string AuthenticatedUserId { get; }
        public string AuthenticatedAccessToken { get; }
        public long AuthenticationDurationMs { get; }
        public Dictionary<string, string> AuthenticationProperties { get; } = new Dictionary<string, string>();

        public static IUserAuthentication CreateAuthenticated(string userId, string sessionToken, SuccessfulAuthenticationMode mode, long duration)
        {
            return new UserAuthentication(UserAuthenticationState.Authenticated, mode, userId, sessionToken, duration);
        }
        
        public static IUserAuthentication CreateUnauthenticated(UserAuthenticationState unauthenticatedState)
        {
            return new UserAuthentication(unauthenticatedState, SuccessfulAuthenticationMode.None, string.Empty, string.Empty);
        }

        
        public static IUserAuthentication CreateCancelled()
        {
            return new UserAuthentication(UserAuthenticationState.Cancelled, SuccessfulAuthenticationMode.None, string.Empty, string.Empty);
        }
    }
}