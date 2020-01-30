using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Client.Azure.Config;
using Microsoft.Identity.Client;

namespace Blauhaus.Auth.Client.Azure.MsalProxy
{
    internal class MsalClientProxy : IMsalClientProxy
    {
        private readonly IAzureActiveDirectoryClientConfig _azureAuthConfig;
        private readonly IAnalyticsService _analyticsService;
        private readonly IPublicClientApplication _authenticationClient;

        public MsalClientProxy(IAzureActiveDirectoryClientConfig azureAuthConfig, IAnalyticsService analyticsService)
        {
            _azureAuthConfig = azureAuthConfig;
            _analyticsService = analyticsService;

            _authenticationClient = PublicClientApplicationBuilder
                .Create(_azureAuthConfig.ApplicationId)
                .WithIosKeychainSecurityGroup(_azureAuthConfig.IosKeychainSecurityGroups)
                .WithB2CAuthority(_azureAuthConfig.AuthoritySignin)
                .Build();
        }

        public async Task<MsalClientResult> AuthenticateSilentlyAsync(CancellationToken cancellationToken)
        {
            try
            {
                var accounts = await _authenticationClient.GetAccountsAsync();

                var authResult = await _authenticationClient
                    .AcquireTokenSilent(_azureAuthConfig.Scopes, accounts.FirstOrDefault())
                    .WithB2CAuthority(_azureAuthConfig.AuthoritySignin)
                    .ExecuteAsync(cancellationToken);

                _analyticsService.Trace("AuthenticationClientService: Silent authentication succeeded for AuthenticatedUserId" + authResult.Account.HomeAccountId, LogSeverity.Information);

                return MsalClientResult.Authenticated(authResult);
            }
            catch (MsalUiRequiredException)
            {
                _analyticsService.Trace("AuthenticationClientService: Silent authentication failed as login is required", LogSeverity.Information);

                return MsalClientResult.RequiresLogin();
            }
            catch (MsalException msalException)
            {
                _analyticsService.Trace( "Silent authentication failed. Error: " + msalException.ErrorCode, LogSeverity.Information);
                
                if (msalException.ErrorCode == "authentication_canceled")
                    return MsalClientResult.Cancelled();
                
                return MsalClientResult.Failed(msalException);
            }
        }

        public async Task<MsalClientResult> LoginAsync(object clientParentView, CancellationToken cancellationToken)
        {
            try
            {
                var authResult = await _authenticationClient
                    .AcquireTokenInteractive(_azureAuthConfig.Scopes)
                    .WithPrompt(Prompt.SelectAccount)
                    .WithParentActivityOrWindow(clientParentView)
                    .ExecuteAsync(cancellationToken);
                    
                _analyticsService.Trace("AuthenticationClientService: Login succeeded for AuthenticatedUserId " + authResult.Account.HomeAccountId, LogSeverity.Information);

                return MsalClientResult.Authenticated(authResult);
            }
            catch (MsalException msalException)
            {
                
                _analyticsService.Trace("AuthenticationClientService: User login failed. Error: " + msalException.ErrorCode, LogSeverity.Warning);

                if (msalException.Message != null && msalException.Message.Contains("AADB2C90118"))
                    return MsalClientResult.RequiresPasswordReset();

                if (msalException.ErrorCode == "authentication_canceled")
                    return MsalClientResult.Cancelled(); 
                
                return MsalClientResult.Failed(msalException);
            }

        }

        public async Task<MsalClientResult> ResetPasswordAsync(object clientParentView, CancellationToken cancellationToken)
        {
            try
            {
                var authResult =  await _authenticationClient
                    .AcquireTokenInteractive(_azureAuthConfig.Scopes)
                    .WithPrompt(Prompt.SelectAccount)
                    .WithParentActivityOrWindow(clientParentView)
                    .WithB2CAuthority(_azureAuthConfig.AuthorityPasswordReset)
                    .ExecuteAsync(cancellationToken);
                        
                _analyticsService.Trace("AuthenticationClientService: User reset password succeeded for AuthenticatedUserId " + authResult.Account.HomeAccountId, LogSeverity.Information);

                return MsalClientResult.Authenticated(authResult);
            }
            catch (MsalException msalException)
            {
                _analyticsService.Trace("AuthenticationClientService: User reset password failed. Error: " + msalException.ErrorCode, LogSeverity.Warning);

                if (msalException.ErrorCode == "authentication_canceled")
                    return MsalClientResult.Cancelled();
                
                if (msalException.Message != null && msalException.Message.Contains("AADB2C90091"))
                    return MsalClientResult.Cancelled();
                
                return MsalClientResult.Failed(msalException);
            }
        }

        public async Task LogoutAsync()
        {
            var accounts = await _authenticationClient.GetAccountsAsync();

            foreach (var account in accounts.ToList())
            {
                await _authenticationClient.RemoveAsync(account);
            }
        }
    }
}