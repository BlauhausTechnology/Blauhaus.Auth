using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.AccessToken;
using Blauhaus.Auth.Abstractions.Models;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Abstractions.User;
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
            IMsalClientProxy msalClientProxy,
            IAuthenticatedAccessToken accessToken)
        {
            _analyticsService = analyticsService;
            _accessToken = accessToken;
            _msalClientProxy = msalClientProxy;
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
                    _analyticsService.TraceInformation(this, $"Manual Login Required because {silentMsalResult.MsalErrorCode}");

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
                _analyticsService.Trace(this,  "No authentication methods were successful", LogSeverity.Warning);
                return UserAuthentication.CreateFailed("No authentication methods were successful", currentAuthMode);
            }
            catch (Exception e)
            {
                if (TryGetFailedAuthentication(e, currentAuthMode, out var failedUserAuthentication))
                {
                    return failedUserAuthentication;
                }
                
                _analyticsService.LogException(this, e);
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
                    _analyticsService.Trace(this, $"{AuthenticationMode.RefreshToken} failed. Login required", LogSeverity.Warning);
                    return UserAuthentication.CreateFailed("MSAL RefreshToken failed. Login required", AuthenticationMode.RefreshToken);
                }

                _analyticsService.Trace(this, "No authentication methods were successful to refresh token", LogSeverity.Warning);
                return UserAuthentication.CreateFailed("No authentication methods were successful", AuthenticationMode.RefreshToken);
            }
            catch (Exception e)
            {
                if (TryGetFailedAuthentication(e, AuthenticationMode.RefreshToken, out var failedUserAuthentication))
                {
                    return failedUserAuthentication;
                }

                _analyticsService.LogException(this, e);
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

                _analyticsService.Trace(this, $"{authenticationModeName} failed due to networking error", LogSeverity.Warning);
                _analyticsService.LogException(this, exception);

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
                
                _analyticsService.Trace(this, $"{authenticationModeName} successful", 
                    LogSeverity.Information, userAuthentication.User.UserId.ToObjectDictionary("UserId"));
                
                return true;
            }

            if (msalClientResult.IsCancelled)
            {
                userAuthentication = UserAuthentication.CreateCancelled(mode);
                _analyticsService.Trace(this, $"{authenticationModeName} cancelled. MSAL state: {msalClientResult.AuthenticationState}", LogSeverity.Information);
                return true;
            }

            if (msalClientResult.IsFailed)
            {
                userAuthentication = UserAuthentication.CreateFailed($"MSAL {authenticationModeName} failed. Error code: {msalClientResult.MsalErrorCode}", mode);
                _analyticsService.Trace(this, $"{authenticationModeName} FAILED: {msalClientResult.MsalErrorCode}. MSAL state: {msalClientResult.AuthenticationState}", 
                    LogSeverity.Warning, msalClientResult.ToObjectDictionary("MSAL result"));
                return true;
            }

            userAuthentication = default;
            return false;
        }

        private IUserAuthentication CreateAuthenticated(MsalClientResult msalClientResult, AuthenticationMode mode)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.ReadJwtToken(msalClientResult.AuthenticationResult.AccessToken);
            var userId = Guid.Parse(msalClientResult.AuthenticationResult.UniqueId);
            string? emailAddress = null;

            var claims = new List<UserClaim>();
            foreach (var claim in token.Claims)
            {
                if (claim.Type.StartsWith("extension_"))
                {
                    claims.Add(new UserClaim(claim.Type.Substring(10), claim.Value));
                }
                else if (claim.Type == "emails")
                {
                    emailAddress = claim.Value;
                }
            }

            var user = new AuthenticatedUser(userId, emailAddress, claims);
            var accessToken = msalClientResult.AuthenticationResult.AccessToken;
            var idToken = msalClientResult.AuthenticationResult.IdToken;

            var userAuthentication = UserAuthentication.CreateAuthenticated(user, accessToken, idToken, mode);

            _accessToken.SetAccessToken("Bearer", userAuthentication.AuthenticatedAccessToken);

            return userAuthentication;
        }

    }
}