using System.Collections.Generic;
using System.Security.Claims;
using Blauhaus.Auth.Abstractions.User;

namespace Blauhaus.Auth.Common.Extensions
{
    public static class UserClaimExtensions
    {
        public static Claim ToClaim(this KeyValuePair<string, string> userClaim) => new(userClaim.Key, userClaim.Value);
        public static Claim ToCustomClaim(this KeyValuePair<string, string> userClaim) => new($"extension_{userClaim.Key}", userClaim.Value);
    }
}