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
            UserClaims = user.UserClaims;
        }

        [JsonConstructor]
        public AuthenticatedUser(Guid userId, string? emailAddress = null, IReadOnlyList<UserClaim>? userClaims = default, string authPolicy = "", string[]? scopes = default)
        {
            UserId = userId;
            EmailAddress = emailAddress;
            AuthPolicy = authPolicy;
            Scopes = scopes ?? Array.Empty<string>();
            UserClaims = userClaims ?? Array.Empty<UserClaim>();
            
            Properties = new Dictionary<string, string>();
            foreach (var userClaim in UserClaims)
            {
                Properties[userClaim.Name] = userClaim.Value;
            }
        }

        public static AuthenticatedUser CreateAdmin(IAuthenticatedUser authenticatedUser)
        {
            return new AuthenticatedUser(authenticatedUser.UserId, authenticatedUser.EmailAddress, new List<UserClaim> { UserClaim.Admin });
        }

        public Guid UserId { get; }
        public string? EmailAddress { get; }
        public string AuthPolicy { get; } = string.Empty;
        public string[] Scopes { get; } = Array.Empty<string>();
        public IReadOnlyList<UserClaim> UserClaims { get; } = Array.Empty<UserClaim>();

        public bool HasClaim(string name)
        {
            var claim = UserClaims.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
            return claim is { Name: { }, Value: { } };
        }

        public bool HasClaimValue(string name, string value)
        {
            var claim = UserClaims.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
            return claim?.Name != default && string.Equals(claim.Value, value, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool TryGetClaimValue(string name, out string value)
        {
            var claim = UserClaims.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
            if (claim is { Name: { }, Value: { } })
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
            foreach (var claim in UserClaims)
            {
                if (claim.Name.ToLower().StartsWith(pefixLowered))
                {
                    claims[claim.Name.Replace(prefix, "")] = claim.Value;
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

            foreach (var userClaim in UserClaims)
            {
                s.Append(userClaim);
            }

            return s.ToString();
        }

        public Dictionary<string, string> Properties { get; }  
    }
}