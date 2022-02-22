using System.Security.Claims;
using Blauhaus.Auth.Abstractions.User;

namespace Blauhaus.Auth.Common.Extensions
{
    public static class UserClaimExtensions
    {
        public static Claim ToClaim(this UserClaim userClaim)
            => new Claim(userClaim.Name, userClaim.Value);
        public static Claim ToCustomClaim(this UserClaim userClaim)
            => new Claim($"extension_{userClaim.Name}", userClaim.Value);
    }
}