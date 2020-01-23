using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Auth.Abstractions.Models;
using Blauhaus.Auth.Client.Azure.MsalProxy;
using Blauhaus.Auth.Tests.UnitTests.Client.AzureAuthenticationClientServiceTests._Base;
using Microsoft.Identity.Client;
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
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.SilentLogin));
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
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.SilentLogin));
        }

        [Test]
        public async Task IF_silent_authentication_fails_SHOULD_return_failed_state()
        {
            //Arrange
            MockMsalClientProxy.Mock.Setup(x => x.AuthenticateSilentlyAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(MsalClientResult.Failed(new MsalException("MSAL Error Code")));

            //Act
            var result = await Sut.LoginAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
            Assert.That(result.AuthenticatedUserId, Is.EqualTo(""));
            Assert.That(result.ErrorMessage, Is.EqualTo($"MSAL {AuthenticationMode.SilentLogin} failed. Error code: MSAL Error Code"));
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.SilentLogin));
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
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ManualLogin));
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
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ManualLogin));
            Assert.That(result.AuthenticatedUserId, Is.EqualTo(""));
        }

        [Test]
        public async Task IF_authentication_requries_login_and_login_fails_SHOULD_return_Failed()
        {
            //Arrange
            MockMsalClientProxy.Mock.Setup(x => x.AuthenticateSilentlyAsync(MockCancelToken))
                .ReturnsAsync(MsalClientResult.RequiresLogin);
            MockMsalClientProxy.Mock.Setup(x => x.LoginAsync(It.IsAny<object>(), MockCancelToken))
                .ReturnsAsync(MsalClientResult.Failed(new MsalException("MSAL Error Code")));

            //Act
            var result = await Sut.LoginAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
            Assert.That(result.AuthenticatedUserId, Is.EqualTo(""));
            Assert.That(result.ErrorMessage, Is.EqualTo($"MSAL {AuthenticationMode.ManualLogin} failed. Error code: MSAL Error Code"));
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ManualLogin));
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
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ResetPassword));
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
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ResetPassword));
        }
        
        [Test]
        public async Task IF_authentication_requries_password_reset_and_it_fails_SHOULD_return_fail()
        {
            //Arrange
            MockMsalClientProxy.Mock.Setup(x => x.AuthenticateSilentlyAsync(MockCancelToken))
                .ReturnsAsync(MsalClientResult.RequiresLogin);
            MockMsalClientProxy.Mock.Setup(x => x.LoginAsync(It.IsAny<object>(), MockCancelToken))
                .ReturnsAsync(MsalClientResult.RequiresPasswordReset);
            MockMsalClientProxy.Mock.Setup(x => x.ResetPasswordAsync(It.IsAny<object>(), MockCancelToken))
                .ReturnsAsync(MsalClientResult.Failed(new MsalException("MSAL Error Code")));

            //Act
            var result = await Sut.LoginAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ResetPassword));
            Assert.That(result.AuthenticatedUserId, Is.EqualTo(""));
            Assert.That(result.ErrorMessage, Is.EqualTo($"MSAL {AuthenticationMode.ResetPassword} failed. Error code: MSAL Error Code"));
        }

        
    }
}