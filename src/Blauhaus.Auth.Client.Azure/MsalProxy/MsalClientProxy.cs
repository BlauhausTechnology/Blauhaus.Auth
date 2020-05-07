using System;
using System.Collections.Generic;
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
        private readonly IPublicClientApplication _authenticationClient;

        private readonly List<KeyValuePair<MsalLogLevel, string>> _msalLogs = new List<KeyValuePair<MsalLogLevel, string>>();

        public MsalClientProxy(IAzureActiveDirectoryClientConfig azureAuthConfig)
        {
            _azureAuthConfig = azureAuthConfig;

            _authenticationClient = PublicClientApplicationBuilder
                .Create(_azureAuthConfig.ApplicationId)
                .WithLogging(Log)
                .WithIosKeychainSecurityGroup(_azureAuthConfig.IosKeychainSecurityGroups)
                .WithB2CAuthority(_azureAuthConfig.AuthoritySignin)
                .Build();
        }

        public async Task<MsalClientResult> AuthenticateSilentlyAsync(CancellationToken cancellationToken, bool forceTokenRefresh = false)
        {
            try
            {
                _msalLogs.Clear();

                var accounts = await _authenticationClient.GetAccountsAsync();

                var authResult = await _authenticationClient
                    .AcquireTokenSilent(_azureAuthConfig.Scopes, accounts.FirstOrDefault())
                    .WithForceRefresh(forceTokenRefresh)
                    .WithB2CAuthority(_azureAuthConfig.AuthoritySignin)
                    .ExecuteAsync(cancellationToken);

                return MsalClientResult.Authenticated(authResult, _msalLogs);
            }
            catch (MsalUiRequiredException e)
            {
                return MsalClientResult.RequiresLogin(e.Classification, _msalLogs);
            }
            catch (MsalException msalException)
            {
                if (msalException.ErrorCode == "authentication_canceled")
                    return MsalClientResult.Cancelled(_msalLogs);
                
                return MsalClientResult.Failed(msalException, _msalLogs);
            }
        }
        
        public async Task<MsalClientResult> LoginAsync(object clientParentView, bool useEmbeddedWwebView, CancellationToken cancellationToken)
        {
            try
            {
                _msalLogs.Clear();
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
                    
                return MsalClientResult.Authenticated(authResult, _msalLogs);
            }
            catch (MsalException msalException)
            {
                
                if (msalException.Message != null && msalException.Message.Contains("AADB2C90118"))
                    return MsalClientResult.RequiresPasswordReset(_msalLogs);

                if (msalException.ErrorCode == "authentication_canceled")
                    return MsalClientResult.Cancelled(_msalLogs); 
                
                return MsalClientResult.Failed(msalException, _msalLogs);
            }

        }

        public async Task<MsalClientResult> ResetPasswordAsync(object clientParentView, bool useEmbeddedWwebView, CancellationToken cancellationToken)
        {
            try
            {
                _msalLogs.Clear();

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

                return MsalClientResult.Authenticated(authResult, _msalLogs);
            }
            catch (MsalException msalException)
            {
                if (msalException.ErrorCode == "authentication_canceled")
                    return MsalClientResult.Cancelled(_msalLogs);
                
                if (msalException.Message != null && msalException.Message.Contains("AADB2C90091"))
                    return MsalClientResult.Cancelled(_msalLogs);
                
                return MsalClientResult.Failed(msalException, _msalLogs);
            }
        }

        public async Task<MsalClientResult> EditProfileAsync(object clientParentView, CancellationToken cancellationToken)
        {
            try
            {
                _msalLogs.Clear();

                var editProfileResult =  await _authenticationClient
                    .AcquireTokenInteractive(_azureAuthConfig.Scopes)
                    .WithPrompt(Prompt.NoPrompt)
                    .WithParentActivityOrWindow(clientParentView)
                    .WithB2CAuthority(_azureAuthConfig.AuthorityPasswordReset)
                    .ExecuteAsync(cancellationToken);
                        
                return MsalClientResult.Authenticated(editProfileResult, _msalLogs);
            }
            catch (MsalException msalException)
            {
                if (msalException.ErrorCode == "authentication_canceled")
                    return MsalClientResult.Cancelled(_msalLogs);
                
                if (msalException.Message != null && msalException.Message.Contains("AADB2C90091"))
                    return MsalClientResult.Cancelled(_msalLogs);
                
                return MsalClientResult.Failed(msalException, _msalLogs);
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
                _msalLogs.Add(new KeyValuePair<MsalLogLevel, string>((MsalLogLevel) level, message));
            }
        }

    }
}