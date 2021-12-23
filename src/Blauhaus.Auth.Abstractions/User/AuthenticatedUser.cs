﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace Blauhaus.Auth.Abstractions.User
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
            Claims = user.Claims;
        }

        [JsonConstructor]
        public AuthenticatedUser(Guid userId, string? emailAddress, IEnumerable<UserClaim> claims, string authPolicy = "", string[]? scopes = default)
        {
            UserId = userId;
            EmailAddress = emailAddress;
            AuthPolicy = authPolicy;
            Scopes = scopes ?? Array.Empty<string>();
            Claims = claims.ToList();
        }

        public static AuthenticatedUser CreateAdmin(IAuthenticatedUser authenticatedUser)
        {
            return new AuthenticatedUser(authenticatedUser.UserId, authenticatedUser.EmailAddress, new List<UserClaim> { UserClaim.Admin });
        }

        public Guid UserId { get; }
        public string? EmailAddress { get; }
        public string AuthPolicy { get; } = string.Empty;
        public string[] Scopes { get; } = Array.Empty<string>();
        public IReadOnlyList<UserClaim> Claims { get; } = Array.Empty<UserClaim>();

        public bool HasClaim(string name)
        {
            var claim = Claims.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
            return claim is { Name: { }, Value: { } };
        }

        public bool HasClaimValue(string name, string value)
        {
            var claim = Claims.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
            return claim?.Name != default && string.Equals(claim.Value, value, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool TryGetClaimValue(string name, out string value)
        {
            var claim = Claims.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
            if (claim is { Name: { }, Value: { } })
            {
                value = claim.Value;
                return true;
            }

            value = string.Empty;
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