using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Auth.Abstractions.Models;
using Blauhaus.Auth.Client.Azure.MsalProxy;
using Blauhaus.Auth.Tests.UnitTests.Client.AzureAuthenticationClientServiceTests._Base;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Client.AzureAuthenticationClientServiceTests
{
    public class LoginAsyncTests : BaseAuthenticationClientServiceTest
    {
        [Test]
        public async Task IF_silent_authentication_succeeds_SHOULD_call_HandleAccessToken_and_return_user()
        {
            //Arrange
            MockMsalClientProxy.Mock.Setup(x => x.AuthenticateSilentlyAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(MockAuthenticatedUserResult);

            //Act
            IUserAuthentication result = await Sut.LoginAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Authenticated));
            Assert.That(result.AuthenticationMode, Is.EqualTo(SuccessfulAuthenticationMode.Silent));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo("authenticatedAccesstoken"));
            Assert.That(result.AuthenticatedUserId, Is.EqualTo("authenticatedUserId"));
            MockAuthenticatedAccessToken.Mock.Verify(x => x.SetAccessToken("Bearer", "authenticatedAccesstoken"));
        }

        [Test]
        public async Task IF_silent_authentication_is_cancelled_SHOULD_return_cancelled()
        {
            //Arrange
            MockMsalClientProxy.Mock.Setup(x => x.AuthenticateSilentlyAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(MsalClientResult.Cancelled);

            //Act
            var result = await Sut.LoginAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Cancelled));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
            Assert.That(result.AuthenticatedUserId, Is.EqualTo(""));
        }

        [Test]
        public async Task IF_authentication_requries_login_and_login_succeeds_SHOULD_call_HandleAccessToken_and_return_user()
        {
            //Arrange
            MockMsalClientProxy.Mock.Setup(x => x.AuthenticateSilentlyAsync(MockCancelToken))
                .ReturnsAsync(MsalClientResult.RequiresLogin());
            MockMsalClientProxy.Mock.Setup(x => x.LoginAsync(It.IsAny<object>(), MockCancelToken))
                .ReturnsAsync(MockAuthenticatedUserResult);

            //Act
            var result = await Sut.LoginAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Authenticated));
            Assert.That(result.AuthenticationMode, Is.EqualTo(SuccessfulAuthenticationMode.Login));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo("authenticatedAccesstoken"));
            Assert.That(result.AuthenticatedUserId, Is.EqualTo("authenticatedUserId"));
            MockAuthenticatedAccessToken.Mock.Verify(x => x.SetAccessToken("Bearer", "authenticatedAccesstoken"));
        }
        
        [Test]
        public async Task IF_authentication_requries_login_and_login_is_cancelled_SHOULD_return_cancellation()
        {
            //Arrange
            MockMsalClientProxy.Mock.Setup(x => x.AuthenticateSilentlyAsync(MockCancelToken))
                .ReturnsAsync(MsalClientResult.RequiresLogin);
            MockMsalClientProxy.Mock.Setup(x => x.LoginAsync(It.IsAny<object>(), MockCancelToken))
                .ReturnsAsync(MsalClientResult.Cancelled);

            //Act
            var result = await Sut.LoginAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Cancelled));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
            Assert.That(result.AuthenticatedUserId, Is.EqualTo(""));
        }

        [Test]
        public async Task IF_authentication_requries_password_reset_and_it_succeeds_SHOULD_call_HandleAccessToken_and_return_user()
        {
            //Arrange
            MockMsalClientProxy.Mock.Setup(x => x.AuthenticateSilentlyAsync(MockCancelToken))
                .ReturnsAsync(MsalClientResult.RequiresLogin);
            MockMsalClientProxy.Mock.Setup(x => x.LoginAsync(It.IsAny<object>(), MockCancelToken))
                .ReturnsAsync(MsalClientResult.RequiresPasswordReset);
            MockMsalClientProxy.Mock.Setup(x => x.ResetPasswordAsync(It.IsAny<object>(), MockCancelToken))
                .ReturnsAsync(MockAuthenticatedUserResult);

            //Act
            var result = await Sut.LoginAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Authenticated));
            Assert.That(result.AuthenticationMode, Is.EqualTo(SuccessfulAuthenticationMode.ResetPassword));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo("authenticatedAccesstoken"));
            Assert.That(result.AuthenticatedUserId, Is.EqualTo("authenticatedUserId"));
            MockAuthenticatedAccessToken.Mock.Verify(x => x.SetAccessToken("Bearer", "authenticatedAccesstoken"));
        }
        
        [Test]
        public async Task IF_authentication_requries_password_reset_and_it_is_cancelled_SHOULD_return_cancelled()
        {
            //Arrange
            MockMsalClientProxy.Mock.Setup(x => x.AuthenticateSilentlyAsync(MockCancelToken))
                .ReturnsAsync(MsalClientResult.RequiresLogin);
            MockMsalClientProxy.Mock.Setup(x => x.LoginAsync(It.IsAny<object>(), MockCancelToken))
                .ReturnsAsync(MsalClientResult.RequiresPasswordReset);
            MockMsalClientProxy.Mock.Setup(x => x.ResetPasswordAsync(It.IsAny<object>(), MockCancelToken))
                .ReturnsAsync(MsalClientResult.Cancelled);

            //Act
            var result = await Sut.LoginAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Cancelled));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
            Assert.That(result.AuthenticatedUserId, Is.EqualTo(""));
        }

        
    }
}