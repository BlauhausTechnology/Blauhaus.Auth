using Blauhaus.Auth.Abstractions.User;

namespace Blauhaus.Auth.Abstractions.Tokens;

public interface IJwtTokenFactory
{
    string GenerateTokenForUser(IAuthenticatedUser authenticatedUser);
}