using System;
using System.Collections.Generic;
using Blauhaus.Common.TestHelpers;
using Microsoft.Identity.Client;

namespace Blauhaus.Auth.Tests.Builders
{
    public class AuthenticationResultBuilder
    {
        private string _accessToken;
        public AuthenticationResultBuilder With_AccessToken(string value)
        {
            _accessToken = value;
            return this;
        }
        
        private string _uniqueId = "uniqueId";
        public AuthenticationResultBuilder With_UniqueId(string value)
        {
            _uniqueId = value;
            return this;
        }
        
        private readonly IAccount _account = new MockBuilder<IAccount>().Object;
        private const bool IsExtendedLifetimeToken = false;
        private readonly DateTimeOffset _expiresOn = DateTimeOffset.Now.AddDays(2);
        private readonly DateTimeOffset _extendedExpiresOn= DateTimeOffset.Now.AddDays(4);
        private const string TenantId = "tenantId";
        public const string IdToken = "idToken";
        private readonly IEnumerable<string> _scopes = new List<string>();
        private readonly Guid _correlationId = Guid.NewGuid();


        public AuthenticationResult Build()
        {
            return new AuthenticationResult(
                _accessToken, 
                IsExtendedLifetimeToken,
                _uniqueId,
                _expiresOn,
                _extendedExpiresOn,
                TenantId,
                _account,
                IdToken,
                _scopes,
                _correlationId);
        }
    }
}