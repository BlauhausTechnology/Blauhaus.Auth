using System.Threading;
using Blauhaus.Auth.Abstractions.Models;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.Auth.TestHelpers.MockBuilders
{
    public class AuthenticationClientServiceMockBuilder : BaseMockBuilder<AuthenticationClientServiceMockBuilder, IAuthenticationClientService>
    {

        public AuthenticationClientServiceMockBuilder()
        {
            Where_LoginAsync_returns(new UserAuthenticationMockBuilder().Object);
            Where_EditProfileAsync_returns(new UserAuthenticationMockBuilder().Object);
            Where_RefreshAccessTokenAsync_returns(new UserAuthenticationMockBuilder().Object);
        }

        public AuthenticationClientServiceMockBuilder Where_LoginAsync_returns(IUserAuthentication result)
        {
            Mock.Setup(x => x.LoginAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            return this;
        }
        public AuthenticationClientServiceMockBuilder Where_TryGetLoggedInUserAsync_returns(IUserAuthentication result)
        {
            Mock.Setup(x => x.TryGetLoggedInUserAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            return this;
        }
        
        public AuthenticationClientServiceMockBuilder Where_EditProfileAsync_returns(IUserAuthentication result)
        {
            Mock.Setup(x => x.EditProfileAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            return this;
        }
        public AuthenticationClientServiceMockBuilder Where_RefreshAccessTokenAsync_returns(IUserAuthentication result)
        {
            Mock.Setup(x => x.RefreshAccessTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            return this;
        } 
    }
}