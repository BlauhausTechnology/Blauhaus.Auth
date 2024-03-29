﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Auth.Common.UserFactory;
using Blauhaus.Auth.Server.Azure.AdalProxy;
using Blauhaus.Auth.Server.Azure.Config;
using Blauhaus.HttpClientService.Abstractions;
using Blauhaus.HttpClientService.Request;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Blauhaus.Auth.Server.Azure.Service
{
    public class AzureAuthenticationServerService : IAzureAuthenticationServerService 
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IAnalyticsLogger<AzureAuthenticationServerService> _logger;
        private readonly IAdalAuthenticationContextProxy _adalAuthenticationContext;

        private readonly string _endpointPrefix;
        private readonly string _endpointPostfix;
        private readonly string _customPropertyNamePrefix;

        public AzureAuthenticationServerService(
            IHttpClientService httpClientService,
            IAzureActiveDirectoryServerConfig config,
            IAdalAuthenticationContextProxy adalAuthenticationContext,
            IAnalyticsLogger<AzureAuthenticationServerService> logger)
        {
            _httpClientService = httpClientService;
            _logger = logger;
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

            await _httpClientService.PatchAsync<string>(request, token);
            _logger.LogInformation("Custom claims {@Claims} set for user {UserId}", claims, userId);
        }

        public async Task<IAuthenticatedUser> GetUserFromAzureAsync(Guid userId, CancellationToken token)
        {
            var accessToken = await _adalAuthenticationContext.AcquireAccessTokenAsync();
            var endpoint = GetGraphEndpointForResource($"/users/{userId.ToString()}");

            var request = new HttpRequestWrapper(endpoint).WithAuthorizationHeader("Bearer", accessToken);

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

            var claims = new Dictionary<string, string>();
            foreach (var rawAzureProperty in azureUserValues)
            {
                if (rawAzureProperty.Key.StartsWith(_customPropertyNamePrefix))
                {
                    var claimType = rawAzureProperty.Key.Replace(_customPropertyNamePrefix, "");
                    claims[claimType] = rawAzureProperty.Value.ToString() ?? string.Empty;
                }
            }

            var user = new AuthenticatedUser(userId, emailAddress, claims);
            
            _logger.LogDebug("User profile retrieved from Azure AD {@AzureUser}", user);

            return user;

        }

        public IAuthenticatedUser ExtractUserFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            string? emailAddress = null;
            var userId = Guid.Empty;
            var userClaims = new Dictionary<string, string>();

            if (claimsPrincipal.Identity is not { IsAuthenticated: true })
            {
                _logger.LogWarning("User is not authenticated");
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            foreach (var claim in claimsPrincipal.Claims)
            {
                if (claim.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")
                {
                    Guid.TryParse(claim.Value, out userId);
                }
                else if (claim.Type == "emails")
                {
                    emailAddress = string.IsNullOrWhiteSpace(claim.Value) ? null : claim.Value;
                }
                else if (claim.Type.StartsWith("extension_"))
                {
                    var claimName = claim.Type.Replace("extension_", "");
                    userClaims[claimName] = claim.Value;
                }
            }

            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Invalid Identity");
                throw new UnauthorizedAccessException("Invalid Identity");
            }

            var user = new AuthenticatedUser(userId, emailAddress, userClaims);

            _logger.LogTrace("User profile extracted from ClaimsPrincipal: " + user.UserId);
            
            return user;
        }

        private string GetGraphEndpointForResource(string resource)
        {
            return _endpointPrefix + resource + _endpointPostfix;
        }

    }
}