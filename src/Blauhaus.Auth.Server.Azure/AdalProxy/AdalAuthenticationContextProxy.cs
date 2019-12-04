using System.Threading.Tasks;
using Blauhaus.Auth.Server.Azure.Config;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Blauhaus.Auth.Server.Azure.AdalProxy
{
    public class AdalAuthenticationContextProxy : IAdalAuthenticationContextProxy
    {
        private readonly IAzureActiveDirectoryServerConfig _config;
        private readonly AuthenticationContext _msalAuthenticationContext;
        private readonly ClientCredential _msalCredential;
        private readonly string _endpointPrefix;
        private readonly string _endpointPostfix;

        public AdalAuthenticationContextProxy(IAzureActiveDirectoryServerConfig config)
        {
            _config = config;
            _msalAuthenticationContext = new AuthenticationContext($"{config.AuthorityBaseUrl}{config.TenantId}");
            _msalCredential = new ClientCredential(config.ApplicationId, config.ClientSecret);
            _endpointPrefix = $"{_config.GraphEndpoint}{config.TenantId}";
            _endpointPostfix = $"?{_config.GraphVersion}";
        }


        public async Task<string> AcquireAccessTokenAsync()
        {
            var authResult = await _msalAuthenticationContext.AcquireTokenAsync(_config.GraphResourceId, _msalCredential);
            return authResult.AccessToken;
        }

        public string GetGraphEndpointForResource(string resource)
        {
            return _endpointPrefix + resource + _endpointPostfix;
        }

    }
}