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

        public async Task<MsalClientResult> AuthenticateSilentlyAsync(CancellationToken cancellationToken, bool forceTokenRefresh = false)
        {
            try
            {
                var accounts = await _authenticationClient.GetAccountsAsync();

                var authResult = await _authenticationClient
                    .AcquireTokenSilent(_azureAuthConfig.Scopes, accounts.FirstOrDefault())
                    .WithForceRefresh(forceTokenRefresh)
                    .WithB2CAuthority(_azureAuthConfig.AuthoritySignin)
                    .ExecuteAsync(cancellationToken);

                return MsalClientResult.Authenticated(authResult);
            }
            catch (MsalUiRequiredException)
            {
                return MsalClientResult.RequiresLogin();
            }
            catch (MsalException msalException)
            {
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
                    
                return MsalClientResult.Authenticated(authResult);
            }
            catch (MsalException msalException)
            {
                
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
                        
                return MsalClientResult.Authenticated(authResult);
            }
            catch (MsalException msalException)
            {
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