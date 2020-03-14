using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Claims;
using Blauhaus.Auth.Abstractions.Extensions;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Auth.Server.Azure.AdalProxy;
using Blauhaus.Auth.Server.Azure.Config;
using Blauhaus.HttpClientService.Abstractions;
using Blauhaus.HttpClientService.Request;
using Blauhaus.Ioc.Abstractions;
using Newtonsoft.Json.Linq;

namespace Blauhaus.Auth.Server.Azure.Service
{
    public class AzureAuthenticationServerService : IAzureAuthenticationServerService 
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IAdalAuthenticationContextProxy _adalAuthenticationContext;

        private readonly string _endpointPrefix;
        private readonly string _endpointPostfix;
        private readonly string _customPropertyNamePrefix;

        public AzureAuthenticationServerService(
            IHttpClientService httpClientService,
            IAzureActiveDirectoryServerConfig config,
            IAdalAuthenticationContextProxy adalAuthenticationContext,
            IAnalyticsService analyticsService)
        {
            _httpClientService = httpClientService;
            _analyticsService = analyticsService;
            _adalAuthenticationContext = adalAuthenticationContext;
            _customPropertyNamePrefix = $"extension_{config.ExtensionsApplicationId}_";
            _endpointPrefix = $"{config.GraphEndpoint}{config.TenantId}";
            _endpointPostfix = $"?{config.GraphVersion}";
        }

        public async Task SetCustomClaimsAsync(Guid userId, Dictionary<string, string> claims, CancellationToken token)
        {
            var accessToken = await _adalAuthenticationContext.AcquireAccessTokenAsync();
            var endpoint = GetGraphEndpointForResource($"/users/{userId.ToString()}");

            var json = new JObject();
            foreach(var claim in claims)
            {
                json.Add($"{_customPropertyNamePrefix}{claim.Key}", claim.Value);
            }
            var request = new HttpRequestWrapper<JObject>(endpoint, json)
                .WithAuthorizationHeader("Bearer", accessToken);

            using (var _ = _analyticsService.ContinueOperation(this, "Update user claims on Azure AD"))
            {
                await _httpClientService.PatchAsync<string>(request, token);
                _analyticsService.Trace(this, "Custom claims set", LogSeverity.Information, json.ToPropertyDictionary("Json")
                    .WithProperty("UserId", userId)
                    .WithProperties(claims));
            }
        }

        public async Task<IAuthenticatedUser> GetUserFromAzureAsync(Guid userId, CancellationToken token)
        {
            var accessToken = await _adalAuthenticationContext.AcquireAccessTokenAsync();
            var endpoint = GetGraphEndpointForResource($"/users/{userId.ToString()}");

            var request = new HttpRequestWrapper(endpoint)
                .WithAuthorizationHeader("Bearer", accessToken);

            using (var _ = _analyticsService.ContinueOperation(this, "Get user profile from Azure AD", userId.ToPropertyDictionary("UserId")))
            {
                var azureUserValues = await _httpClientService.GetAsync<Dictionary<string, object>>(request, token);
            
                string? emailAddress = null;

                if (azureUserValues.TryGetValue("signInNames", out var signInNames))
                {

                    if (signInNames is JArray signInNameProperties)
                    {
                        foreach (var signInNameProperty in signInNameProperties)
                        {
                            var key = (string)signInNameProperty.First.Value<JProperty>().Value;
                            if (key == "emailAddress")
                            {
                                emailAddress = signInNameProperty.Last.Value<JProperty>().Value.ToString();
                            }
                        }
                    }
                }

                var claims = new List<Claim>();
                foreach (var rawAzureProperty in azureUserValues)
                {
                    if (rawAzureProperty.Key.StartsWith(_customPropertyNamePrefix))
                    {
                        var claimType = rawAzureProperty.Key.Replace(_customPropertyNamePrefix, "");
                        claims.Add(new Claim(claimType, rawAzureProperty.Value.ToString()));
                    }
                }

                var user = new AuthenticatedUser(userId, emailAddress, claims);
                
                _analyticsService.Trace(this, "User profile retrieved from Azure AD", LogSeverity.Verbose, user.ToPropertyDictionary("AzureADUser"));

                return user;
            }

        }

        public IAuthenticatedUser ExtractUserFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            string? emailAddress = null;
            Guid userId;
            var claims = claimsPrincipal.Claims;
            
            if (!claimsPrincipal.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var objectIdentifier = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypesExtended.ObjectIdentifierClaimType);
            if (objectIdentifier == null || string.IsNullOrEmpty(objectIdentifier.Value))
            {
                throw new UnauthorizedAccessException("Invalid identity");
            }

            userId = Guid.Parse(objectIdentifier.Value);
            if (userId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("Invalid identity");
            }

            var emails = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "emails");
            if (emails != null && !string.IsNullOrEmpty(emails.Value))
            {
                emailAddress = emails.Value;
            }

            var user = new AuthenticatedUser(userId, emailAddress, claims);

            _analyticsService.Trace(this, "User profile extracted from ClaimsPrincipal", 
                LogSeverity.Verbose, user.ToPropertyDictionary("AzureADUser"));
            
            return user;
        }

        private string GetGraphEndpointForResource(string resource)
        {
            return _endpointPrefix + resource + _endpointPostfix;
        }

    }
}