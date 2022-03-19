using System.Collections.Generic;
using Blauhaus.Auth.Client.Azure.Config;
using Microsoft.Identity.Client;

namespace Blauhaus.Auth.Client.Azure.MsalProxy
{
    public class MsalClientResult
    {
        private MsalClientResult(MsalAuthenticationState authenticationState, AuthenticationResult authenticationResult, string errorCode = "")
        {
            AuthenticationState = authenticationState;
            AuthenticationResult = authenticationResult;
            MsalErrorCode = errorCode;
        }

        public bool IsAuthenticated => AuthenticationState == MsalAuthenticationState.Authenticated;
        public bool IsCancelled => AuthenticationState == MsalAuthenticationState.Cancelled;
        public bool IsFailed => AuthenticationState == MsalAuthenticationState.Failed;

        public MsalAuthenticationState AuthenticationState { get; }
        public AuthenticationResult AuthenticationResult { get; }
        public string MsalErrorCode { get; }

        public static MsalClientResult Authenticated(AuthenticationResult authenticationResult)
        {
            return new MsalClientResult(MsalAuthenticationState.Authenticated, authenticationResult);
        }        
        
        public static MsalClientResult RequiresLogin(UiRequiredExceptionClassification errorCode)
        {
            return new MsalClientResult(MsalAuthenticationState.RequiresLogin, null, errorCode.ToString());
        }

        public static MsalClientResult RequiresPasswordReset()
        {
            return new MsalClientResult(MsalAuthenticationState.RequiresPasswordReset, null);
        }

        public static MsalClientResult Cancelled()
        {
            return new MsalClientResult(MsalAuthenticationState.Cancelled, null);
        }
        public static MsalClientResult Failed(MsalException e)
        {
            return new MsalClientResult(MsalAuthenticationState.Failed, null, e.ErrorCode);
        }
    }
}