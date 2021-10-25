using Blauhaus.Auth.Abstractions.User;

namespace Blauhaus.Auth.Abstractions.Extensions
{
    public static class AuthenticatedUserExtensions
    {
        public static bool IsAdminUser(this IAuthenticatedUser user)
        {
            return user.HasClaimValue("Role", "Admin");
        }
    }
}