using System;
using Blauhaus.Auth.Abstractions.Tokens;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.Auth.TestHelpers.MockBuilders;

public class JwtTokenFactoryMockBuilder : BaseMockBuilder<JwtTokenFactoryMockBuilder, IJwtTokenFactory>
{
    public JwtTokenFactoryMockBuilder()
    {
        Where_GenerateTokenForUser_returns(Guid.NewGuid().ToString());
    }

    public JwtTokenFactoryMockBuilder Where_GenerateTokenForUser_returns(string token)
    {
        Mock.Setup(x => x.GenerateTokenForUser(It.IsAny<IAuthenticatedUser>()))
            .Returns(token);
        return this;
    }
}