using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

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
    }
}