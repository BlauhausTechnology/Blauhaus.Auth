﻿using System.Collections.Generic;
using Blauhaus.Common.Abstractions;

namespace Blauhaus.Auth.Abstractions.User
{
    public interface IAuthenticatedUser : IHasUserId, IHasProperties
    {
        string? EmailAddress { get; }

        public string AuthPolicy { get; }
        public string[] Scopes { get; }

        bool HasClaim(string name);
        bool HasClaimValue(string name, string value);
        bool TryGetClaimValue(string name, out string value);
        Dictionary<string, string> GetClaimValuesByPrefix(string prefix);
    }
    
}