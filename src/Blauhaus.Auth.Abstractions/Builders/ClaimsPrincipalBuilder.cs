using System.Collections.Generic;
using System.Security.Claims;
using Blauhaus.Auth.Abstractions.Claims;

namespace Blauhaus.Auth.Abstractions.Builders
{
    public class ClaimsPrincipalBuilder
    {
        private readonly ClaimsPrincipal _claimsPrincipal;
        private string _authSchemeName = string.Empty;
        private readonly List<Claim> _claims = new List<Claim>();


        public ClaimsPrincipalBuilder(bool isAuthenticated = true)
        {
            _claimsPrincipal = new ClaimsPrincipal();
            if (isAuthenticated)
            {
                WithIsAuthenticatedTrue();
            }
        }

        public ClaimsPrincipalBuilder WithIsAuthenticatedTrue(string scheme = "Bearer")
        {
            _authSchemeName = scheme;
            return this;
        }
        
        public ClaimsPrincipalBuilder WithIsAuthenticatedFalse()
        {
            _authSchemeName = string.Empty;
            return this;
        }

        public ClaimsPrincipalBuilder With_NameIdentifier(string nameIdentifier)
        {
            _claims.Add(new Claim(ClaimTypes.NameIdentifier, nameIdentifier));
            return this;
        }
        public ClaimsPrincipalBuilder With_ObjectId(string objectIdentifier)
        {
            _claims.Add(new Claim(ClaimTypesExtended.ObjectIdentifierClaimType, objectIdentifier));
            return this;
        }

        public ClaimsPrincipalBuilder With_Claim(string claimType, string claimValue)
        {
            _claims.Add(new Claim(claimType, claimValue));
            return this;
        }

        public ClaimsPrincipal Build()
        {
            var claimsIdentity = string.IsNullOrEmpty(_authSchemeName)
                ? new ClaimsIdentity()              //IsAuthenticated = false
                : new ClaimsIdentity("Bearer");     //IsAuthenticated = true

            claimsIdentity.AddClaims(_claims);

            _claimsPrincipal.AddIdentity(claimsIdentity);
            return _claimsPrincipal;
        }
    }
}