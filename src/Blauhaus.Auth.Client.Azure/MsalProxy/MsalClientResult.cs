using Microsoft.Identity.Client;

namespace Blauhaus.Auth.Client.Azure.MsalProxy
{
    public class MsalClientResult
    {
        private MsalClientResult(MsalAuthenticationState authenticationState, AuthenticationResult authenticationResult)
        {
            AuthenticationState = authenticationState;
            AuthenticationResult = authenticationResult;
        }

        public bool IsAuthenticated => AuthenticationState == MsalAuthenticationState.Authenticated;
        public bool IsCancelled => AuthenticationState == MsalAuthenticationState.Cancelled;

        public MsalAuthenticationState AuthenticationState { get; }
        public AuthenticationResult AuthenticationResult { get; }

        public static MsalClientResult Authenticated(AuthenticationResult authenticationResult)
        {
            return new MsalClientResult(MsalAuthenticationState.Authenticated, authenticationResult);
        }        
        
        public static MsalClientResult RequiresLogin()
        {
            return new MsalClientResult(MsalAuthenticationState.RequiresLogin, null);
        }

        public static MsalClientResult RequiresPasswordReset()
        {
            return new MsalClientResult(MsalAuthenticationState.RequiresPasswordReset, null);
        }

        public static MsalClientResult Cancelled()
        {
            return new MsalClientResult(MsalAuthenticationState.Cancelled, null);
        }
    }
}