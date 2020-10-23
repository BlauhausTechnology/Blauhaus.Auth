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
            var user = new AuthenticatedUserMockBuilder().Object;
            Where_ExtractFromClaimsPrincipal_returns(user);
            Where_ExtractFromClaimsPrincipal_returns(user);
        }

        public AuthenticatedUserFactoryMockBuilder Where_ExtractFromClaimsPrincipal_returns(IAuthenticatedUser value)
        {
            Mock.Setup(x => x.ExtractFromClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).Returns(Response.Success(value));
            return this;
        }
        public AuthenticatedUserFactoryMockBuilder Where_ExtractFromClaimsPrincipal_fails(Error error)
        {
            Mock.Setup(x => x.ExtractFromClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).Returns(Response.Failure<IAuthenticatedUser>(error));
            return this;
        }
        public AuthenticatedUserFactoryMockBuilder Where_ExtractFromJwtToken_returns(IAuthenticatedUser value)
        {
            Mock.Setup(x => x.ExtractFromJwtToken(It.IsAny<string>())).Returns(Response.Success(value));
            return this;
        }
        public AuthenticatedUserFactoryMockBuilder Where_ExtractFromJwtToken_fails(Error error)
        {
            Mock.Setup(x => x.ExtractFromJwtToken(It.IsAny<string>())).Returns(Response.Failure<IAuthenticatedUser>(error));
            return this;
        }
    }
}