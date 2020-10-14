using System.Security.Claims;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Blauhaus.TestHelpers.MockBuilders;
using CSharpFunctionalExtensions;
using Moq;

namespace Blauhaus.Auth.TestHelpers.MockBuilders
{
    public class AuthenticatedUserFactoryMockBuilder : BaseMockBuilder<AuthenticatedUserFactoryMockBuilder, IAuthenticatedUserFactory>
    {

        public AuthenticatedUserFactoryMockBuilder()
        {
            Where_Create_returns(new AuthenticatedUserMockBuilder().Object);
        }

        public AuthenticatedUserFactoryMockBuilder Where_Create_returns(IAuthenticatedUser value)
        {
            Mock.Setup(x => x.Create(It.IsAny<ClaimsPrincipal>())).Returns(Response.Success(value));
            return this;
        }
        public AuthenticatedUserFactoryMockBuilder Where_Create_fails(Error error)
        {
            Mock.Setup(x => x.Create(It.IsAny<ClaimsPrincipal>())).Returns(Response.Failure<IAuthenticatedUser>(error));
            return this;
        }
    }
}