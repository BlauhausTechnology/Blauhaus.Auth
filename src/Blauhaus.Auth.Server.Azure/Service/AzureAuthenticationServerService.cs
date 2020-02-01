using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Server.Azure.AdalProxy;
using Blauhaus.Auth.Server.Azure.Config;
using Blauhaus.Auth.Server.Azure.User;
using Blauhaus.HttpClientService.Abstractions;
using Blauhaus.HttpClientService.Request;
using Blauhaus.Ioc.Abstractions;
using Newtonsoft.Json.Linq;

namespace Blauhaus.Auth.Server.Azure.Service
{
    public class AzureAuthenticationServerService<TUser> : IAzureAuthenticationServerService<TUser> 
        where TUser : class, IAzureActiveDirectoryUser
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAnalyticsService _analyticsService;
        private readonly IAdalAuthenticationContextProxy _adalAuthenticationContext;

        private readonly string _endpointPrefix;
        private readonly string _endpointPostfix;
        private readonly string _customPropertyNamePrefix;

        public AzureAuthenticationServerService(
            IHttpClientService httpClientService,
            IAzureActiveDirectoryServerConfig config,
            IServiceProvider serviceProvider,
            IAnalyticsService analyticsService)
        {
            _httpClientService = httpClientService;
            var config1 = config;
            _serviceProvider = serviceProvider;
            _analyticsService = analyticsService;
            _adalAuthenticationContext = (IAdalAuthenticationContextProxy) serviceProvider.GetService(typeof(IAdalAuthenticationContextProxy));
            _customPropertyNamePrefix = $"extension_{config1.ExtensionsApplicationId}_";
            _endpointPrefix = $"{config1.GraphEndpoint}{config.TenantId}";
            _endpointPostfix = $"?{config1.GraphVersion}";
        }

        public async Task SetCustomClaimAsync(string userObjectId, string propertyName, string value, CancellationToken token)
        {
            var accessToken = await _adalAuthenticationContext.AcquireAccessTokenAsync();
            var endpoint = GetGraphEndpointForResource($"/users/{userObjectId}");
            var json = new JObject { [$"{_customPropertyNamePrefix}{propertyName}"] = value};

            var request = new HttpRequestWrapper<JObject>(endpoint, json)
                .WithAuthorizationHeader("Bearer", accessToken);
            
            using (var _ = _analyticsService.ContinueOperation("Update user claim on Azure AD"))
            {
                await _httpClientService.PatchAsync<string>(request, token);
                _analyticsService.Trace("Custom claim set", LogSeverity.Information, new Dictionary<string, object>
                {
                    {"Claims", json }
                });
            }
        }

        public async Task SetCustomClaimsAsync(string userObjectId, Dictionary<string, string> claims, CancellationToken token)
        {
            var accessToken = await _adalAuthenticationContext.AcquireAccessTokenAsync();
            var endpoint = GetGraphEndpointForResource($"/users/{userObjectId}");

            var json = new JObject();
            foreach(var claim in claims)
            {
                json.Add($"{_customPropertyNamePrefix}{claim.Key}", claim.Value);
            }
            var request = new HttpRequestWrapper<JObject>(endpoint, json)
                .WithAuthorizationHeader("Bearer", accessToken);

            using (var _ = _analyticsService.ContinueOperation("Update user claims on Azure AD"))
            {
                await _httpClientService.PatchAsync<string>(request, token);
                _analyticsService.Trace("Custom claim set", LogSeverity.Information, new Dictionary<string, object>
                {
                    {"Claims", json }
                });
            }
        }

        public async Task<TUser> GetUserAsync(string userObjectId, CancellationToken token)
        {
            var accessToken = await _adalAuthenticationContext.AcquireAccessTokenAsync();
            var endpoint = GetGraphEndpointForResource($"/users/{userObjectId}");

            var request = new HttpRequestWrapper(endpoint)
                .WithAuthorizationHeader("Bearer", accessToken);

            using (var _ = _analyticsService.ContinueOperation("Get user profile from Azure AD"))
            {
                var azureUserValues = await _httpClientService.GetAsync<Dictionary<string, object>>(request, token);
            
                var user = (TUser)_serviceProvider.GetService(typeof(TUser));
                user.Initialize(azureUserValues);

                var customProperties = new Dictionary<string, object>();
                foreach (var rawAzureProperty in azureUserValues)
                {
                    if (rawAzureProperty.Key.StartsWith(_customPropertyNamePrefix))
                    {
                        var originalPropertyName = rawAzureProperty.Key.Replace(_customPropertyNamePrefix, "");
                        customProperties[originalPropertyName] = rawAzureProperty.Value;
                    }
                }

                user.PopulateCustomProperties(customProperties);

                _analyticsService.Trace("User profile retrieved", LogSeverity.Verbose, new Dictionary<string, object>
                {
                    {"AzureADUser", user }
                });

                return user;
            }

        }

        public TUser ExtractUser(ClaimsPrincipal claimsPrincipal)
        {
            var user = (TUser)_serviceProvider.GetService(typeof(TUser));
            user.Initialize(claimsPrincipal);
            
            _analyticsService.Trace("User profile extracted from ClaimsPrincipal", LogSeverity.Verbose, new Dictionary<string, object> {{"AzureADUser", user}});
            
            return user;
        }


        private string GetGraphEndpointForResource(string resource)
        {
            return _endpointPrefix + resource + _endpointPostfix;
        }

    }
}