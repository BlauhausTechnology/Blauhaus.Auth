using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using Blauhaus.Auth.Abstractions.Builders;
using Blauhaus.Auth.Abstractions.Claims;
using Blauhaus.Auth.Abstractions.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Blauhaus.Auth.Server.Azure.User
{
    public sealed class AuthenticatedUser : IAuthenticatedUser
    {
        public AuthenticatedUser()
        {
        }

        public AuthenticatedUser(Guid userId, string? emailAddress, IEnumerable<Claim> claims)
        {
            UserId = userId;
            EmailAddress = emailAddress;
            Claims = claims.ToList();
        }


        public Guid UserId { get; private set; }
        public string? EmailAddress { get; private set; }
        public IList<Claim> Claims { get; } = new List<Claim>();
    }
}