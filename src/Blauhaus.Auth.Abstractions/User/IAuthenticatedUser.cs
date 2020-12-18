using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Blauhaus.Auth.Abstractions.User
{
    public interface IAuthenticatedUser
    {
        Guid UserId { get; }   
        string? EmailAddress { get; }

        public string AuthPolicy { get; }
        public string[] Scopes { get; }

        IReadOnlyList<UserClaim> Claims { get; }
        bool HasClaim(string name);
        bool HasClaimValue(string name, string value);
        bool TryGetClaimValue(string name, out string value);


    }
}