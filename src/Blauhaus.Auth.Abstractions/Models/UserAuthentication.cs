using System.Collections.Generic;
using Blauhaus.Auth.Abstractions.User;

namespace Blauhaus.Auth.Abstractions.Models
{
    public class UserAuthentication : IUserAuthentication
    {
        private UserAuthentication(
            UserAuthenticationState authenticationState, 
            AuthenticationMode mode, 
            IAuthenticatedUser? user, 
            string authToken,
            string idToken,
            string errorMessage = "")
        {
            AuthenticatedAccessToken = authToken;
            AuthenticationState = authenticationState;
            AuthenticationMode = mode;
            User = user;
            AuthenticatedIdToken = idToken;
            ErrorMessage = errorMessage;
        }

        public bool IsAuthenticated => AuthenticationState ==
                                       UserAuthenticationState.Authenticated &&
                                       User != null &&
                                       !string.IsNullOrEmpty(AuthenticatedAccessToken);

        public UserAuthenticationState AuthenticationState { get; }
        public AuthenticationMode AuthenticationMode { get; }
        public string AuthenticatedAccessToken { get; }
        public string AuthenticatedIdToken { get; }
        public string ErrorMessage { get; }
        public IAuthenticatedUser User { get; }


        public static IUserAuthentication CreateAuthenticated(IAuthenticatedUser user, string accessToken, string idToken, AuthenticationMode mode)
        {
            return new UserAuthentication(UserAuthenticationState.Authenticated, mode, user, accessToken, idToken);
        }
        
        public static IUserAuthentication CreateCancelled(AuthenticationMode mode)
        {
            return new UserAuthentication(UserAuthenticationState.Cancelled, mode, null, string.Empty, string.Empty);
        }

        public static IUserAuthentication CreateFailed(string errorMessage, AuthenticationMode mode)
        {
            return new UserAuthentication(UserAuthenticationState.Failed, mode, null, string.Empty, string.Empty, errorMessage);
        }
    }
}