using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.ClientAuthenticationHandlers;
using Blauhaus.Auth.Abstractions.Extensions;
using Blauhaus.Auth.Abstractions.Models;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Client.Azure.MsalProxy;
using Blauhaus.Ioc.Abstractions;

namespace Blauhaus.Auth.Client.Azure.Service
{
    public class AzureAuthenticationClientService : IAuthenticationClientService
    {
        private readonly IAuthenticatedAccessToken _accessToken;
        private readonly IAnalyticsService _analyticsService;
        private readonly IMsalClientProxy _msalClientProxy;

        public AzureAuthenticationClientService(
            IAnalyticsService analyticsService,
            IIocService iocService,
            IAuthenticatedAccessToken accessToken)
        {
            _analyticsService = analyticsService;
            _accessToken = accessToken;
            _msalClientProxy = iocService.Resolve<IMsalClientProxy>();
        }

        public static object NativeParentView { get; set; }

        public async Task<IUserAuthentication> LoginAsync(CancellationToken cancellationToken)
        {

            var currentAuthMode = AuthenticationMode.SilentLogin;

            try
            {
                var silentMsalResult = await _msalClientProxy.AuthenticateSilentlyAsync(cancellationToken);

                if (TryGetCompletedUserAuthentication(silentMsalResult, AuthenticationMode.SilentLogin, out var completedSilentLoginAuthentication))
                {
                    return completedSilentLoginAuthentication;
                }

                if (silentMsalResult.AuthenticationState == MsalAuthenticationState.RequiresLogin)
                {
                    currentAuthMode = AuthenticationMode.ManualLogin;
                    var loginMsalResult = await _msalClientProxy.LoginAsync(NativeParentView, cancellationToken);

                    if (TryGetCompletedUserAuthentication(loginMsalResult, AuthenticationMode.ManualLogin, out var completedManualLoginAuthentication))
                    {
                        return completedManualLoginAuthentication;
                    }

                    if (loginMsalResult.AuthenticationState == MsalAuthenticationState.RequiresPasswordReset)
                    {
                        currentAuthMode = AuthenticationMode.ResetPassword;
                        var resetPasswordMsalResult = await _msalClientProxy.ResetPasswordAsync(NativeParentView, cancellationToken);

                        if (TryGetCompletedUserAuthentication(resetPasswordMsalResult, AuthenticationMode.ResetPassword, out var completedResetPasswordAuthentication))
                        {
                            return completedResetPasswordAuthentication;
                        }
                    }
                }
                _analyticsService.Trace( "No authentication methods were successful", LogSeverity.Warning);
                return UserAuthentication.CreateFailed("No authentication methods were successful", currentAuthMode);
            }
            catch (Exception e)
            {
                if (TryGetFailedAuthentication(e, currentAuthMode, out var failedUserAuthentication))
                {
                    return failedUserAuthentication;
                }
                
                _analyticsService.LogException(e);
                throw;
            }
        }

        public async Task<IUserAuthentication> RefreshAccessTokenAsync(CancellationToken cancellationToken)
        {

            try
            {
                var silentMsalResult = await _msalClientProxy.AuthenticateSilentlyAsync(cancellationToken, true);

                if (TryGetCompletedUserAuthentication(silentMsalResult, AuthenticationMode.RefreshToken, out var completedRefreshTokenAuthentication))
                {
                    return completedRefreshTokenAuthentication;
                }

                if (silentMsalResult.AuthenticationState == MsalAuthenticationState.RequiresLogin)
                {
                    _analyticsService.Trace($"{AuthenticationMode.RefreshToken} failed. Login required", LogSeverity.Warning);
                    return UserAuthentication.CreateFailed("MSAL RefreshToken failed. Login required", AuthenticationMode.RefreshToken);
                }

                _analyticsService.Trace("No authentication methods were successful to refresh token", LogSeverity.Warning);
                return UserAuthentication.CreateFailed("No authentication methods were successful", AuthenticationMode.RefreshToken);
            }
            catch (Exception e)
            {
                if (TryGetFailedAuthentication(e, AuthenticationMode.RefreshToken, out var failedUserAuthentication))
                {
                    return failedUserAuthentication;
                }

                _analyticsService.LogException(e);
                throw;
            }
        }

        public async Task LogoutAsync()
        {
            await _msalClientProxy.LogoutAsync();
            _accessToken.Clear();
        }

        private bool TryGetFailedAuthentication(Exception exception, AuthenticationMode mode, out IUserAuthentication failedUserAuthentication)
        {
            var authenticationModeName = Enum.GetName(typeof(AuthenticationMode), mode);

            if (exception is HttpRequestException ||
                exception.Message != null && exception.Message.Contains("Unable to resolve host")) //Android
            {
                
                failedUserAuthentication = UserAuthentication.CreateFailed($"MSAL {authenticationModeName} failed. Networking error", mode);

                _analyticsService.Trace( $"{authenticationModeName} failed due to networking error", LogSeverity.Warning);
                _analyticsService.LogException(exception);

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
                
                _analyticsService.Trace( $"{authenticationModeName} successful for {userAuthentication.AuthenticatedUserId}", 
                    LogSeverity.Information, userAuthentication.AuthenticatedUserId.ToPropertyDictionary("AuthenticatedUserId"));
                
                return true;
            }

            if (msalClientResult.IsCancelled)
            {
                userAuthentication = UserAuthentication.CreateCancelled(mode);
                _analyticsService.Trace($"{authenticationModeName} cancelled. MSAL state: {msalClientResult.AuthenticationState}", LogSeverity.Information);
                return true;
            }

            if (msalClientResult.IsFailed)
            {
                userAuthentication = UserAuthentication.CreateFailed($"MSAL {authenticationModeName} failed. Error code: {msalClientResult.MsalErrorCode}", mode);
                _analyticsService.Trace( $"{authenticationModeName} FAILED: {msalClientResult.MsalErrorCode}. MSAL state: {msalClientResult.AuthenticationState}", 
                    LogSeverity.Warning, msalClientResult.ToPropertyDictionary("MSAL result"));
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

    }
}