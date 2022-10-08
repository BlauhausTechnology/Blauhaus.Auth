using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Blauhaus.Auth.Abstractions.User;

namespace Blauhaus.Auth.Common.UserFactory
{
    public class AuthenticatedUser : IAuthenticatedUser
    {

        public AuthenticatedUser()
        {
        }

        public AuthenticatedUser(IAuthenticatedUser user)
        {
            UserId = user.UserId;
            EmailAddress = user.EmailAddress;
            AuthPolicy = user.AuthPolicy;
            Scopes = user.Scopes;
            Properties = user.Properties; 
        }
         
        [JsonConstructor]
        public AuthenticatedUser(Guid userId, string? emailAddress = null, IDictionary<string, string>? userClaims = default, string authPolicy = "", string[]? scopes = default)
        {
            UserId = userId;
            EmailAddress = emailAddress;
            AuthPolicy = authPolicy;
            Scopes = scopes ?? Array.Empty<string>();
            
            Properties = new Dictionary<string, string>();
            if (userClaims is not null)
            {
                foreach (var userClaim in userClaims)
                {
                    Properties[userClaim.Key] = userClaim.Value;
                }
            }
        }

        public Guid UserId { get; }
        public string? EmailAddress { get; }
        public string AuthPolicy { get; } = string.Empty;
        public string[] Scopes { get; } = Array.Empty<string>();
        
 
        public bool HasClaim(string name)
        {
            var claim = Properties.FirstOrDefault(x => string.Equals(x.Key, name, StringComparison.InvariantCultureIgnoreCase));
            return claim is { Key : { }, Value: { } };
        }

        public bool HasClaimValue(string name, string value)
        {
            var claim = Properties.FirstOrDefault(x => 
                string.Equals(x.Key, name, StringComparison.InvariantCultureIgnoreCase) && 
                string.Equals(x.Value, value, StringComparison.InvariantCultureIgnoreCase));

            return claim is { Key : { }, Value: { } };
        }

        public bool TryGetClaimValue(string name, out string value)
        {
            var claim = Properties.FirstOrDefault(x => string.Equals(x.Key, name, StringComparison.InvariantCultureIgnoreCase));
            if (claim is { Key: { }, Value: { } })
            {
                value = claim.Value;
                return true;
            }

            value = string.Empty;
            return false;
        }

        public Dictionary<string, string> GetClaimValuesByPrefix(string prefix)
        {
            var pefixLowered = prefix.ToLower();
            var claims = new Dictionary<string, string>();
            foreach (var claim in Properties)
            {
                if (claim.Key.ToLower().StartsWith(pefixLowered))
                {
                    claims[claim.Key.Replace(prefix, "")] = claim.Value;
                }
            }
            return claims;
        }

        public override string ToString()
        {
            var s = new StringBuilder()
                .Append(UserId.ToString());

            if (EmailAddress != null)
                s.Append($" [{EmailAddress}] ");

            foreach (var userClaim in Properties)
            {
                s.Append(userClaim);
            }

            return s.ToString();
        }

        public Dictionary<string, string> Properties { get; }  
    }
}