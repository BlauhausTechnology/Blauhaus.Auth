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
        private readonly ITimeService _timeService;
        private readonly IAuthenticatedAccessToken _accessToken;
        private readonly IMsalClientProxy _msalClientProxy;

        public static object NativeParentView { get; set; }

        public AzureAuthenticationClientService(
            ILogService logService,
            IIocService iocService,
            ITimeService timeService,
            IAuthenticatedAccessToken accessToken)
        {
            _logService = logService;
            _timeService = timeService;
            _accessToken = accessToken;
            _msalClientProxy = iocService.Resolve<IMsalClientProxy>();
        }

        public async Task<IUserAuthentication> LoginAsync(CancellationToken cancellationToken)
        {
            var start = _timeService.CurrentUtcTimestampMs;

            var silentMsalResult = await _msalClientProxy.AuthenticateSilentlyAsync(cancellationToken);

            if (silentMsalResult.IsAuthenticated)
                return CreateAuthenticated(silentMsalResult, SuccessfulAuthenticationMode.Silent, start);

            if (silentMsalResult.IsCancelled)
                return UserAuthentication.CreateCancelled();

            if (silentMsalResult.AuthenticationState == MsalAuthenticationState.RequiresLogin)
            {
                var loginMsalResult = await _msalClientProxy.LoginAsync(NativeParentView, cancellationToken);
                
                if (loginMsalResult.IsAuthenticated)
                    return CreateAuthenticated(loginMsalResult, SuccessfulAuthenticationMode.Login, start);

                if (loginMsalResult.IsCancelled)
                    return UserAuthentication.CreateCancelled();

                if (loginMsalResult.AuthenticationState == MsalAuthenticationState.RequiresPasswordReset)
                {
                    var resetPasswordMsalResult = await _msalClientProxy.ResetPasswordAsync(NativeParentView, cancellationToken);
                    
                    if (resetPasswordMsalResult.IsCancelled)
                        return UserAuthentication.CreateCancelled();

                    if (resetPasswordMsalResult.IsAuthenticated)
                        return CreateAuthenticated(resetPasswordMsalResult, SuccessfulAuthenticationMode.ResetPassword, start);
                }
            }

            return UserAuthentication.CreateUnauthenticated(UserAuthenticationState.Error);
        }

        private IUserAuthentication CreateAuthenticated(MsalClientResult msalClientResult, SuccessfulAuthenticationMode mode, long startMs)
        {
            
            var duration = _timeService.CurrentUtcTimestampMs - startMs;

            var userAuthentication = UserAuthentication.CreateAuthenticated(
                msalClientResult.AuthenticationResult.UniqueId,
                msalClientResult.AuthenticationResult.AccessToken, mode, duration);

            _accessToken.SetAccessToken("Bearer", userAuthentication.AuthenticatedAccessToken);

            _logService.LogMessage(LogLevel.Trace, $"Authentication successful. Mode: {mode.ToString()}  Duration(ms): {duration}");

            return userAuthentication;

        }

        public async Task LogoutAsync()
        {
            await _msalClientProxy.LogoutAsync();
            _accessToken.Clear();
        }
    }
}