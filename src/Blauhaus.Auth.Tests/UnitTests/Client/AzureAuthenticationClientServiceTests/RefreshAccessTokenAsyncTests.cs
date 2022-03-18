using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Models;
using Blauhaus.Auth.Client.Azure.MsalProxy;
using Blauhaus.Auth.Tests.UnitTests.Client.AzureAuthenticationClientServiceTests.Base;
using Microsoft.Identity.Client;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Client.AzureAuthenticationClientServiceTests
{
    public class RefreshAccessTokenAsyncTests : BaseAuthenticationClientServiceTest
    {

        [Test]
        public async Task SHOULD_attempt_silent_auth_with_ForceRefresh_true()
        {
            //Arrange
            MockMsalClientProxy.Where_AuthenticateSilentlyAsync_returns(MockAuthenticatedUserResult);

            //Act
            await Sut.RefreshAccessTokenAsync(MockCancelToken);

            //Assert
            MockMsalClientProxy.Mock.Verify(x => x.AuthenticateSilentlyAsync(It.IsAny<CancellationToken>(), true));
        }

        [Test]
        public async Task IF_silent_authentication_succeeds_SHOULD_set_UserIds_and_return_user()
        {
            //Arrange
            MockMsalClientProxy.Where_AuthenticateSilentlyAsync_returns(MockAuthenticatedUserResult);

            //Act
            var result = await Sut.RefreshAccessTokenAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Authenticated));
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.RefreshToken));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(AccessToken));
            Assert.That(result.User.UserId, Is.EqualTo(UserId));
            var userType = result.User.Claims.FirstOrDefault(x => x.Name == "UserType");
            Assert.That(userType, Is.Not.Null);
            Assert.That(userType.Value, Is.EqualTo("Admin"));
            MockAuthenticatedAccessToken.Mock.Verify(x => x.SetAccessToken("Bearer", AccessToken));
            MockLogger.VerifySetValue("UserId", UserId);
        }

        [Test]
        public async Task IF_silent_authentication_is_cancelled_SHOULD_return_cancelled()
        {
            //Arrange
            MockMsalClientProxy.Where_AuthenticateSilentlyAsync_returns(MsalClientResult.Cancelled(MockLogs));

            //Act
            var result = await Sut.RefreshAccessTokenAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Cancelled));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
            Assert.That(result.User, Is.Null);
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.RefreshToken));
        }

        [Test]
        public async Task IF_silent_authentication_fails_SHOULD_return_failed_state()
        {
            //Arrange
            var fail = MsalClientResult.Failed(new MsalException("MSAL Error Code"), MockLogs);
            MockMsalClientProxy.Where_AuthenticateSilentlyAsync_returns(fail);

            //Act
            var result = await Sut.RefreshAccessTokenAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
            Assert.That(result.User, Is.Null);
            Assert.That(result.ErrorMessage, Is.EqualTo($"MSAL {AuthenticationMode.RefreshToken} failed. Error code: MSAL Error Code"));
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.RefreshToken));
        }

        [Test]
        public async Task IF_silent_authentication_throws_HttpRequestException_SHOULD_return_failed_state()
        {
            //Arrange
            var exception = new HttpRequestException("Network issue");
            MockMsalClientProxy.Where_AuthenticateSilentlyAsync_throws(exception);

            //Act
            var result = await Sut.RefreshAccessTokenAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
            Assert.That(result.User, Is.Null);
            Assert.That(result.ErrorMessage!.StartsWith("MSAL RefreshToken failed. Networking error"));
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.RefreshToken));
        }

        [Test]
        public async Task IF_silent_authentication_throws_weird_android_network_error_SHOULD_return_failed_state()
        {
            //Arrange
            var exception = new Exception("Unable to resolve host \"minegameauth.b2clogin.com\": No address associated with hostname");
            MockMsalClientProxy.Where_AuthenticateSilentlyAsync_throws(exception);

            //Act
            var result = await Sut.RefreshAccessTokenAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
            Assert.That(result.User, Is.Null);
            Assert.That(result.ErrorMessage!.StartsWith("MSAL RefreshToken failed. Networking error"));
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.RefreshToken));

        }


        [Test]
        public async Task IF_authentication_requries_login_SHOUD_return_fail()
        {
            //Arrange
            MockMsalClientProxy
                .Where_AuthenticateSilentlyAsync_returns(MsalClientResult
                    .RequiresLogin(UiRequiredExceptionClassification.ConsentRequired, MockLogs));

            //Act
            var result = await Sut.RefreshAccessTokenAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.RefreshToken));
            Assert.That(result.ErrorMessage, Is.EqualTo("MSAL RefreshToken failed. Login required"));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
            Assert.That(result.User, Is.Null);
        }


    }
}