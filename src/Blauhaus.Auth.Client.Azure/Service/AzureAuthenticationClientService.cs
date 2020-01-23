using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Auth.Abstractions.ClientAuthenticationHandlers;
using Blauhaus.Auth.Abstractions.Models;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Client.Azure.Config;
using Blauhaus.Auth.Client.Azure.MsalProxy;
using Blauhaus.Common.Time.Service;
using Blauhaus.Ioc.Abstractions;
using Blauhaus.Loggers.Common.Abstractions;

namespace Blauhaus.Auth.Client.Azure.Service
{
    public class AzureAuthenticationClientService : IAuthenticationClientService
    {
        private readonly ILogService _logService;
        private readonly IAuthenticatedAccessToken _accessToken;
        private readonly IMsalClientProxy _msalClientProxy;

        public static object NativeParentView { get; set; }

        public AzureAuthenticationClientService(
            ILogService logService,
            IIocService iocService,
            IAuthenticatedAccessToken accessToken)
        {
            _logService = logService;
            _accessToken = accessToken;
            _msalClientProxy = iocService.Resolve<IMsalClientProxy>();
        }

        public async Task<IUserAuthentication> LoginAsync(CancellationToken cancellationToken)
        {
            MsalClientResult silentMsalResult;

            try
            {
                silentMsalResult = await _msalClientProxy.AuthenticateSilentlyAsync(cancellationToken);

                if (TryGetCompletedUserAuthentication(silentMsalResult, AuthenticationMode.SilentLogin, out var completedSilentLoginAuthentication))
                    return completedSilentLoginAuthentication;
            }
            catch (Exception e)
            {
                if (TryGetFailedAuthentication(e, AuthenticationMode.SilentLogin, out var failedUserAuthentication))
                {
                    return failedUserAuthentication;
                }

                throw;
            }

            if (silentMsalResult.AuthenticationState == MsalAuthenticationState.RequiresLogin)
            {
                var loginMsalResult = await _msalClientProxy.LoginAsync(NativeParentView, cancellationToken);

                if (TryGetCompletedUserAuthentication(loginMsalResult, AuthenticationMode.ManualLogin, out var completedManualLoginAuthentication))
                {
                    return completedManualLoginAuthentication;
                }

                if (loginMsalResult.AuthenticationState == MsalAuthenticationState.RequiresPasswordReset)
                {
                    var resetPasswordMsalResult = await _msalClientProxy.ResetPasswordAsync(NativeParentView, cancellationToken);
                    
                    if (TryGetCompletedUserAuthentication(resetPasswordMsalResult, AuthenticationMode.ResetPassword, out var completedResetPasswordAuthentication))
                    {
                        return completedResetPasswordAuthentication;
                    }
                }
            }

            return UserAuthentication.CreateFailed("No authentication methods were successful", AuthenticationMode.None);
        }

        private bool TryGetFailedAuthentication(Exception exception, AuthenticationMode mode, out IUserAuthentication failedUserAuthentication)
        {
            var authenticationModeName = Enum.GetName(typeof(AuthenticationMode), mode);

            if (exception is HttpRequestException ||
                exception.Message != null && exception.Message.Contains("Unable to resolve host"))  //Android
            {
                 failedUserAuthentication = UserAuthentication.CreateFailed($"MSAL {authenticationModeName} failed. Networking error", mode);
                 return true;
            }

            failedUserAuthentication = null;
            return false;
        }
        
        private bool TryGetCompletedUserAuthentication(MsalClientResult msalClientResult, AuthenticationMode mode, out IUserAuthentication userAuthentication)
        {
            var authenticationModeName = Enum.GetName(typeof(AuthenticationMode), mode);

            if (msalClientResult.IsAuthenticated)
            {
                userAuthentication = CreateAuthenticated(msalClientResult, mode);
                _logService.LogMessage(LogLevel.Trace, $"{authenticationModeName} successful");
                return true;
            }

            if (msalClientResult.IsCancelled)
            {
                userAuthentication = UserAuthentication.CreateCancelled(mode);
                _logService.LogMessage(LogLevel.Trace, $"{authenticationModeName} cancelled");
                return true;
            }

            if (msalClientResult.IsFailed)
            {
                userAuthentication = UserAuthentication.CreateFailed($"MSAL {authenticationModeName} failed. Error code: {msalClientResult.MsalErrorCode}", mode);
                _logService.LogMessage(LogLevel.Trace, $"{authenticationModeName} FAILED: {msalClientResult.MsalErrorCode}");
                return true;
            }

            userAuthentication = default;
            return false;
        }

        private IUserAuthentication CreateAuthenticated(MsalClientResult msalClientResult, AuthenticationMode mode)
        {
            
            var userAuthentication = UserAuthentication.CreateAuthenticated(
                msalClientResult.AuthenticationResult.UniqueId,
                msalClientResult.AuthenticationResult.AccessToken, mode);

            _accessToken.SetAccessToken("Bearer", userAuthentication.AuthenticatedAccessToken);

            return userAuthentication;

        }

        public async Task LogoutAsync()
        {
            await _msalClientProxy.LogoutAsync();
            _accessToken.Clear();
        }
    }
}