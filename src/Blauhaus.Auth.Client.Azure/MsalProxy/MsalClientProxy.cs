using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Auth.Client.Azure.Config;
using Blauhaus.Loggers.Common.Abstractions;
using Microsoft.Identity.Client;
using LogLevel = Blauhaus.Loggers.Common.Abstractions.LogLevel;

namespace Blauhaus.Auth.Client.Azure.MsalProxy
{
    internal class MsalClientProxy : IMsalClientProxy
    {
        private readonly IAzureActiveDirectoryClientConfig _azureAuthConfig;
        private readonly ILogService _logService;
        private readonly IPublicClientApplication _authenticationClient;

        public MsalClientProxy(IAzureActiveDirectoryClientConfig azureAuthConfig, ILogService logService)
        {
            _azureAuthConfig = azureAuthConfig;
            _logService = logService;

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

                _logService.LogMessage(LogLevel.Trace, "Silent authentication succeeded");

                return MsalClientResult.Authenticated(authResult);
            }
            catch (MsalUiRequiredException)
            {
                _logService.LogMessage(LogLevel.Trace, "Silent authentication failed as login is required");

                return MsalClientResult.RequiresLogin();
            }
            catch (MsalException msalException)
            {
                _logService.LogMessage(LogLevel.Trace, "Silent authentication failed " + msalException.ErrorCode);

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
                    
                _logService.LogMessage(LogLevel.Trace, "User login succeeded");

                return MsalClientResult.Authenticated(authResult);
            }
            catch (MsalException msalException)
            {
                
                _logService.LogMessage(LogLevel.Trace, "User login failed " + msalException.ErrorCode);

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
                        
                _logService.LogMessage(LogLevel.Trace, "User reset password succeeded");
                return MsalClientResult.Authenticated(authResult);
            }
            catch (MsalException msalException)
            {
                _logService.LogMessage(LogLevel.Trace, "User reset password failed");

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