using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Auth.Client.Azure.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using LogLevel = Microsoft.Identity.Client.LogLevel;

namespace Blauhaus.Auth.Client.Azure.MsalProxy
{
    internal class MsalClientProxy : IMsalClientProxy
    {
        private readonly IAnalyticsLogger<MsalClientProxy> _logger;
        private readonly IAzureActiveDirectoryClientConfig _azureAuthConfig;
        private readonly IPublicClientApplication _authenticationClient;


        public MsalClientProxy(
            IAnalyticsLogger<MsalClientProxy> logger,
            IAzureActiveDirectoryClientConfig azureAuthConfig)
        {
            _logger = logger;
            _azureAuthConfig = azureAuthConfig;

            _authenticationClient = PublicClientApplicationBuilder
                .Create(_azureAuthConfig.ApplicationId)
                .WithLogging(Log)
                .WithRedirectUri($"msal{_azureAuthConfig.ApplicationId}://auth")
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
            catch (MsalUiRequiredException e)
            {
                return MsalClientResult.RequiresLogin(e.Classification);
            }
            catch (MsalException msalException)
            {
                if (msalException.ErrorCode == "authentication_canceled")
                    return MsalClientResult.Cancelled();
                
                return MsalClientResult.Failed(msalException);
            }
        }
        
        public async Task<MsalClientResult> LoginAsync(object clientParentView, bool useEmbeddedWwebView, CancellationToken cancellationToken)
        {
            try
            {
                var client = _authenticationClient
                    .AcquireTokenInteractive(_azureAuthConfig.Scopes)
                    .WithPrompt(Prompt.SelectAccount)
                    .WithParentActivityOrWindow(clientParentView);

                if (useEmbeddedWwebView)
                {
                    //cannot pass in false if not required as UWP breaks
                    client.WithUseEmbeddedWebView(true);
                }

                var authResult = await client
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

        public async Task<MsalClientResult> ResetPasswordAsync(object clientParentView, bool useEmbeddedWwebView, CancellationToken cancellationToken)
        {
            try
            {
                var client = _authenticationClient
                    .AcquireTokenInteractive(_azureAuthConfig.Scopes)
                    .WithPrompt(Prompt.SelectAccount)
                    .WithParentActivityOrWindow(clientParentView)
                    .WithB2CAuthority(_azureAuthConfig.AuthorityPasswordReset);

                if (useEmbeddedWwebView)
                {
                    client.WithUseEmbeddedWebView(true);
                }

                var authResult =  await client
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

        public async Task<MsalClientResult> EditProfileAsync(object clientParentView, CancellationToken cancellationToken)
        {
            try
            {
                var editProfileResult =  await _authenticationClient
                    .AcquireTokenInteractive(_azureAuthConfig.Scopes)
                    .WithPrompt(Prompt.NoPrompt)
                    .WithParentActivityOrWindow(clientParentView)
                    .WithB2CAuthority(_azureAuthConfig.AuthorityPasswordReset)
                    .ExecuteAsync(cancellationToken);
                        
                return MsalClientResult.Authenticated(editProfileResult);
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

        
        private void Log(LogLevel level, string message, bool containspii)
        {
            if (!containspii)
            {
                if(level == LogLevel.Always) _logger.LogTrace(message);
                else if(level == LogLevel.Info) _logger.LogInformation(message);
                else if(level == LogLevel.Warning) _logger.LogWarning(message);
                else if(level == LogLevel.Error) _logger.LogError(message);
            }
        }
         

    }
}