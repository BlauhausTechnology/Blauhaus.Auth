using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.AccessToken;
using Blauhaus.Auth.Abstractions.Models;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Auth.Client.Azure.Config;
using Blauhaus.Auth.Client.Azure.MsalProxy;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Microsoft.Extensions.Logging;

namespace Blauhaus.Auth.Client.Azure.Service
{
    public class AzureAuthenticationClientService : IAuthenticationClientService
    {
        private readonly IAnalyticsLogger<AzureAuthenticationClientService> _logger;
        private readonly IAuthenticatedAccessToken _accessToken;
        private readonly IAzureActiveDirectoryClientConfig _config;
        private readonly IMsalClientProxy _msalClientProxy;

        public AzureAuthenticationClientService(
            IAnalyticsLogger<AzureAuthenticationClientService> logger,
            IMsalClientProxy msalClientProxy,
            IAuthenticatedAccessToken accessToken,
            IAzureActiveDirectoryClientConfig config)
        {
            _logger = logger;
            _accessToken = accessToken;
            _config = config;
            _msalClientProxy = msalClientProxy;
        }

        public static object NativeParentView { get; set; } = null!;

        public async Task<IUserAuthentication> TryGetLoggedInUserAsync(CancellationToken cancellationToke)
        {
            try
            {
                using var _ = _logger.LogTimed(LogLevel.Information, "Tried to get logged in user");

                var silentMsalResult = await _msalClientProxy.AuthenticateSilentlyAsync(CancellationToken.None);

                if (TryGetCompletedUserAuthentication(silentMsalResult, AuthenticationMode.SilentLogin, out var completedSilentLoginAuthentication))
                {
                    return completedSilentLoginAuthentication;
                }

                _logger.LogWarning("No authentication methods were successful");
                return UserAuthentication.CreateFailed("No authentication methods were successful", AuthenticationMode.SilentLogin);
            }
            catch (Exception e)
            {
                if (TryGetFailedAuthentication(e, AuthenticationMode.SilentLogin, out var failedUserAuthentication))
                {
                    return failedUserAuthentication;
                }

                _logger.LogError(Error.Unexpected(), e);
                throw;
            }
        }

        public async Task<IUserAuthentication> LoginAsync(CancellationToken cancellationToken)
        {

            var currentAuthMode = AuthenticationMode.SilentLogin;

            try
            {
                using var _ = _logger.LogTimed(LogLevel.Information, "Logged in user");
                var silentMsalResult = await _msalClientProxy.AuthenticateSilentlyAsync(cancellationToken);

                if (TryGetCompletedUserAuthentication(silentMsalResult, AuthenticationMode.SilentLogin, out var completedSilentLoginAuthentication))
                {
                    return completedSilentLoginAuthentication;
                }

                if (silentMsalResult.AuthenticationState == MsalAuthenticationState.RequiresLogin)
                {
                    _logger.LogInformation("Manual Login Required because {MsalErrorCode}", silentMsalResult.MsalErrorCode);

                    currentAuthMode = AuthenticationMode.ManualLogin;
                    var loginMsalResult = await _msalClientProxy.LoginAsync(NativeParentView, _config.UseEmbeddedWebView, cancellationToken);

                    if (TryGetCompletedUserAuthentication(loginMsalResult, AuthenticationMode.ManualLogin, out var completedManualLoginAuthentication))
                    {
                        return completedManualLoginAuthentication;
                    }

                    if (loginMsalResult.AuthenticationState == MsalAuthenticationState.RequiresPasswordReset)
                    {
                        currentAuthMode = AuthenticationMode.ResetPassword;
                        var resetPasswordMsalResult = await _msalClientProxy.ResetPasswordAsync(NativeParentView, _config.UseEmbeddedWebView, cancellationToken);

                        if (TryGetCompletedUserAuthentication(resetPasswordMsalResult, AuthenticationMode.ResetPassword, out var completedResetPasswordAuthentication))
                        {
                            return completedResetPasswordAuthentication;
                        }
                    }
                }
                _logger.LogWarning("No authentication methods were successful");
                return UserAuthentication.CreateFailed("No authentication methods were successful", currentAuthMode);
            }
            catch (Exception e)
            {
                if (TryGetFailedAuthentication(e, currentAuthMode, out var failedUserAuthentication))
                {
                    return failedUserAuthentication;
                }
                
                _logger.LogError(Error.Unexpected(), e);
                throw;
            }
        }

        public async Task<IUserAuthentication> RefreshAccessTokenAsync(CancellationToken cancellationToken)
        {

            try
            {
                using var _ = _logger.LogTimed(LogLevel.Information, "Refreshed access token");

                var silentMsalResult = await _msalClientProxy.AuthenticateSilentlyAsync(cancellationToken, true);

                if (TryGetCompletedUserAuthentication(silentMsalResult, AuthenticationMode.RefreshToken, out var completedRefreshTokenAuthentication))
                {
                    return completedRefreshTokenAuthentication;
                }

                if (silentMsalResult.AuthenticationState == MsalAuthenticationState.RequiresLogin)
                {
                    _logger.LogInformation("Refreshing auth token failed: {MsalErrorCode}. Login required", silentMsalResult.MsalErrorCode);
                    return UserAuthentication.CreateFailed("MSAL RefreshToken failed. Login required", AuthenticationMode.RefreshToken);
                }

                _logger.LogWarning("No authentication methods were successful to refresh token");
                return UserAuthentication.CreateFailed("No authentication methods were successful", AuthenticationMode.RefreshToken);
            }
            catch (Exception e)
            {
                if (TryGetFailedAuthentication(e, AuthenticationMode.RefreshToken, out var failedUserAuthentication))
                {
                    return failedUserAuthentication;
                }

                _logger.LogError(Error.Unexpected(), e);
                throw;
            }
        }

        public async Task<IUserAuthentication> EditProfileAsync(CancellationToken cancellationToken)
        {
            
            try
            {
                
                using var _ = _logger.LogTimed(LogLevel.Information, "Edited profile");

                var editProfileResult = await _msalClientProxy.EditProfileAsync(NativeParentView, cancellationToken);

                if (TryGetCompletedUserAuthentication(editProfileResult, AuthenticationMode.EditProfile, out var completed))
                {
                    return completed;
                }

                if (editProfileResult.AuthenticationState == MsalAuthenticationState.RequiresLogin)
                {
                    _logger.LogWarning("EditProfile failed: {MsalErrorCode}. Login required", editProfileResult.MsalErrorCode);
                    return UserAuthentication.CreateFailed($"MSAL {AuthenticationMode.EditProfile} failed. Login required", AuthenticationMode.EditProfile);
                }

                _logger.LogWarning("No authentication methods were successful to refresh token");
                return UserAuthentication.CreateFailed("No authentication methods were successful", AuthenticationMode.RefreshToken);
            }
            catch (Exception e)
            {
                if (TryGetFailedAuthentication(e, AuthenticationMode.EditProfile, out var failedUserAuthentication))
                {
                    return failedUserAuthentication;
                }
                
                _logger.LogError(Error.Unexpected(), e);
                throw;
            }
        }

        public async Task LogoutAsync()
        {
            using var _ = _logger.LogTimed(LogLevel.Information, "Logged out user");
            await _msalClientProxy.LogoutAsync();
            _accessToken.Clear();
            _logger.SetValue("UserId", Guid.Empty);
        }

        private bool TryGetFailedAuthentication(Exception exception, AuthenticationMode mode, out IUserAuthentication failedUserAuthentication)
        {
            var authenticationModeName = Enum.GetName(typeof(AuthenticationMode), mode);

            if (exception is HttpRequestException ||
                exception.Message != null && exception.Message.Contains("Unable to resolve host")) //Android
            {
                failedUserAuthentication = UserAuthentication.CreateFailed($"MSAL {authenticationModeName} failed. Networking error ({exception.Message})", mode);

                _logger.LogWarning(exception, "{AuthenticationMode} failed due to networking error", authenticationModeName);

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
                _logger.LogDebug("{AuthenticationMode} successful", authenticationModeName);
                
                return true;
            }

            if (msalClientResult.IsCancelled)
            {
                userAuthentication = UserAuthentication.CreateCancelled(mode);
                _logger.LogInformation("{AuthenticationMode} cancelled. MSAL state: {AuthenticationState}", authenticationModeName, msalClientResult.AuthenticationState);
                return true;
            }

            if (msalClientResult.IsFailed)
            {
                userAuthentication = UserAuthentication.CreateFailed($"MSAL {authenticationModeName} failed. Error code: {msalClientResult.MsalErrorCode}", mode);
                _logger.LogWarning("{AuthenticationMode} FAILED: {MsalErrorCode}. MSAL state: {AuthenticationState}", 
                    authenticationModeName, msalClientResult.MsalErrorCode, msalClientResult.AuthenticationState);
                
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

            _logger.SetValue("UserId", userAuthentication.User!.UserId);
            _accessToken.SetAccessToken("Bearer", userAuthentication.AuthenticatedAccessToken!);

            return userAuthentication;
        }

    }
}