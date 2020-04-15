using System.Collections.Generic;
using Blauhaus.Auth.Client.Azure.Config;
using Microsoft.Identity.Client;

namespace Blauhaus.Auth.Client.Azure.MsalProxy
{
    public class MsalClientResult
    {
        private MsalClientResult(MsalAuthenticationState authenticationState, AuthenticationResult authenticationResult, List<KeyValuePair<MsalLogLevel, string>> logs, string errorCode = "")
        {
            AuthenticationState = authenticationState;
            AuthenticationResult = authenticationResult;
            AuthenticationLogs = logs;
            MsalErrorCode = errorCode;
        }

        public bool IsAuthenticated => AuthenticationState == MsalAuthenticationState.Authenticated;
        public bool IsCancelled => AuthenticationState == MsalAuthenticationState.Cancelled;
        public bool IsFailed => AuthenticationState == MsalAuthenticationState.Failed;

        public MsalAuthenticationState AuthenticationState { get; }
        public AuthenticationResult AuthenticationResult { get; }
        public List<KeyValuePair<MsalLogLevel, string>> AuthenticationLogs { get; }
        public string MsalErrorCode { get; }

        public static MsalClientResult Authenticated(AuthenticationResult authenticationResult, List<KeyValuePair<MsalLogLevel, string>> logs)
        {
            return new MsalClientResult(MsalAuthenticationState.Authenticated, authenticationResult, logs);
        }        
        
        public static MsalClientResult RequiresLogin(UiRequiredExceptionClassification errorCode, List<KeyValuePair<MsalLogLevel, string>> logs)
        {
            return new MsalClientResult(MsalAuthenticationState.RequiresLogin, null, logs, errorCode.ToString());
        }

        public static MsalClientResult RequiresPasswordReset(List<KeyValuePair<MsalLogLevel, string>> logs)
        {
            return new MsalClientResult(MsalAuthenticationState.RequiresPasswordReset, null, logs);
        }

        public static MsalClientResult Cancelled(List<KeyValuePair<MsalLogLevel, string>> logs)
        {
            return new MsalClientResult(MsalAuthenticationState.Cancelled, null, logs);
        }
        public static MsalClientResult Failed(MsalException e, List<KeyValuePair<MsalLogLevel, string>> logs)
        {
            return new MsalClientResult(MsalAuthenticationState.Failed, null, logs, e.ErrorCode);
        }
    }
}