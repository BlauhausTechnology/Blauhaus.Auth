using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Blauhaus.Auth.Abstractions.User
{
    public sealed class AuthenticatedUser : IAuthenticatedUser
    {
        public AuthenticatedUser()
        {
        }

        public AuthenticatedUser(Guid userId, string? emailAddress, IEnumerable<UserClaim> claims)
        {
            UserId = userId;
            EmailAddress = emailAddress;
            Claims = claims.ToList();
        }


        public Guid UserId { get; }
        public string? EmailAddress { get; }
        public IReadOnlyList<UserClaim> Claims { get; }

        public bool HasClaim(string name)
        {
            var claim = Claims.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
            return claim.Name != default && claim.Value != default;
        }

        public bool HasClaimValue(string name, string value)
        {
            var claim = Claims.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
            return claim.Name != default && string.Equals(claim.Value, value, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool TryGetClaimValue(string name, out string value)
        {
            var claim = Claims.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
            if (claim.Name != default && claim.Value != default)
            {
                value = claim.Value;
                return true;
            }

            value = null;
            return false;
        }

        public override string ToString()
        {
            var s = new StringBuilder()
                .Append(UserId.ToString());

            if (EmailAddress != null)
                s.Append($" [{EmailAddress}] ");

            foreach (var userClaim in Claims)
            {
                s.Append(userClaim);
            }

            return s.ToString();
        }
    }
}