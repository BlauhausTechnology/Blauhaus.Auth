using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Auth.Abstractions.Models;
using Blauhaus.Auth.Client.Azure.MsalProxy;
using Blauhaus.Auth.Tests.UnitTests.Client.AzureAuthenticationClientServiceTests.Base;
using Microsoft.Identity.Client;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Client.AzureAuthenticationClientServiceTests
{
    public class LoginAsyncTests : BaseAuthenticationClientServiceTest
    {

        public class SilentLogin : LoginAsyncTests
        {
            [Test]
            public async Task IF_silent_authentication_succeeds_SHOULD_set_UserIds_and_return_user()
            {
                //Arrange
                MockMsalClientProxy.Where_AuthenticateSilentlyAsync_returns(MockAuthenticatedUserResult);

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Authenticated));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.SilentLogin));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(AccessToken));
                Assert.That(result.User!.UserId, Is.EqualTo(UserId));
                Assert.That(result.User.EmailAddress, Is.EqualTo("adrian@maxxor.com"));
                var userType = result.User.UserClaims.FirstOrDefault(x => x.Name == "UserType");
                Assert.That(userType, Is.Not.Null);
                Assert.That(userType.Value, Is.EqualTo("Admin"));
                MockAuthenticatedAccessToken.Mock.Verify(x => x.SetAccessToken("Bearer", AccessToken));
                MockLogger.VerifySetValue("UserId", UserId);
            }

            [Test]
            public async Task SHOULD_log_info_matching_trace_level()
            {
                //Arrange
                MockMsalClientProxy.Where_AuthenticateSilentlyAsync_returns(MockAuthenticatedUserResult);

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Authenticated));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.SilentLogin));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(AccessToken));
                Assert.That(result.User.UserId, Is.EqualTo(UserId));
                Assert.That(result.User.EmailAddress, Is.EqualTo("adrian@maxxor.com"));
                var userType = result.User.UserClaims.FirstOrDefault(x => x.Name == "UserType");
                Assert.That(userType, Is.Not.Null);
                Assert.That(userType.Value, Is.EqualTo("Admin"));
                MockAuthenticatedAccessToken.Mock.Verify(x => x.SetAccessToken("Bearer", AccessToken)); 
            }

            [Test]
            public async Task IF_silent_authentication_is_cancelled_SHOULD_return_cancelled()
            {
                //Arrange
                MockMsalClientProxy.Where_AuthenticateSilentlyAsync_returns(MsalClientResult.Cancelled());

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Cancelled));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
                Assert.That(result.User, Is.Null);
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.SilentLogin)); 
            }

            [Test]
            public async Task IF_silent_authentication_fails_SHOULD_return_failed_state()
            {
                //Arrange
                var fail = MsalClientResult.Failed(new MsalException("MSAL Error Code"));
                MockMsalClientProxy.Where_AuthenticateSilentlyAsync_returns(fail);

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
                Assert.That(result.User, Is.Null);
                Assert.That(result.ErrorMessage, Is.EqualTo($"MSAL {AuthenticationMode.SilentLogin} failed. Error code: MSAL Error Code"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.SilentLogin)); 
            }

            [Test]
            public async Task IF_silent_authentication_throws_HttpRequestException_SHOULD_return_failed_state()
            {
                //Arrange
                var exception = new HttpRequestException("Network issue");
                MockMsalClientProxy.Where_AuthenticateSilentlyAsync_throws(exception);

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
                Assert.That(result.User, Is.Null);
                Assert.That(result.ErrorMessage!.StartsWith("MSAL SilentLogin failed. Networking error"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.SilentLogin));
            }

            [Test]
            public async Task IF_silent_authentication_throws_weird_android_network_error_SHOULD_return_failed_state()
            {
                //Arrange
                var e = new Exception("Unable to resolve host \"minegameauth.b2clogin.com\": No address associated with hostname");
                MockMsalClientProxy.Where_AuthenticateSilentlyAsync_throws(e);

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
                Assert.That(result.User, Is.Null);
                Assert.That(result.ErrorMessage!.StartsWith("MSAL SilentLogin failed. Networking error"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.SilentLogin));

            }

        }

        public class ManualLogin : LoginAsyncTests
        {
            public override void Setup()
            {
                base.Setup();

                MockMsalClientProxy
                    .Where_AuthenticateSilentlyAsync_returns(MsalClientResult
                        .RequiresLogin(UiRequiredExceptionClassification.ConsentRequired));
            }
             

            [Test]
            public async Task IF_authentication_requries_login_and_login_succeeds_SHOULD_set_UserIds_and_return_user()
            {
                //Arrange
                MockConfig.With(x => x.UseEmbeddedWebView, true);
                MockMsalClientProxy.Where_LoginAsync_returns(MockAuthenticatedUserResult);

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Authenticated));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ManualLogin));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(AccessToken));
                Assert.That(result.User.UserId, Is.EqualTo(UserId));
                MockAuthenticatedAccessToken.Mock.Verify(x => x.SetAccessToken("Bearer", AccessToken)); 
                MockLogger.VerifySetValue("UserId", UserId);
                MockMsalClientProxy.Mock.Verify(x => x.LoginAsync(It.IsAny<object>(), true, It.IsAny<CancellationToken>()));
            }

            [Test]
            public async Task IF_authentication_requries_login_and_login_is_cancelled_SHOULD_return_cancellation()
            {
                //Arrange
                MockMsalClientProxy.Where_LoginAsync_returns(MsalClientResult.Cancelled());

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Cancelled));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ManualLogin));
                Assert.That(result.User, Is.Null);
            }

            [Test]
            public async Task IF_authentication_requries_login_and_login_fails_SHOULD_return_Failed()
            {
                //Arrange
                var fail = MsalClientResult.Failed(new MsalException("MSAL Error Code"));
                MockMsalClientProxy.Where_LoginAsync_returns(fail);

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
                Assert.That(result.User, Is.Null);
                Assert.That(result.ErrorMessage, Is.EqualTo($"MSAL {AuthenticationMode.ManualLogin} failed. Error code: MSAL Error Code"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ManualLogin));
            }

            [Test]
            public async Task IF_manual_login_throws_HttpRequestException_SHOULD_return_failed_state()
            {
                //Arrange
                var exception = new HttpRequestException("Network issue");
                MockMsalClientProxy.Where_LoginAsync_throws(exception);

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
                Assert.That(result.User, Is.Null);
                Assert.That(result.ErrorMessage, Is.EqualTo("MSAL ManualLogin failed. Networking error (Network issue)"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ManualLogin));
            }

            [Test]
            public async Task IF_manual_login_throws_weird_android_network_error_SHOULD_return_failed_state()
            {
                //Arrange
                var exception = new Exception("Unable to resolve host \"minegameauth.b2clogin.com\": No address associated with hostname");
                MockMsalClientProxy.Where_LoginAsync_throws(exception);

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
                Assert.That(result.User, Is.Null);
                Assert.That(result.ErrorMessage!.StartsWith("MSAL ManualLogin failed. Networking error"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ManualLogin));
            }
        }

        public class ResetPassword : LoginAsyncTests
        {
            
            public override void Setup()
            {
                base.Setup();

                MockMsalClientProxy
                    .Where_AuthenticateSilentlyAsync_returns(MsalClientResult
                        .RequiresLogin(UiRequiredExceptionClassification.ConsentRequired))
                    .Where_LoginAsync_returns(MsalClientResult.RequiresPasswordReset());
            }

            [Test]
            public async Task IF_authentication_requries_password_reset_and_it_succeeds_SHOULD_set_UserIds_and_return_user()
            {
                //Arrange
                MockMsalClientProxy.Where_ResetPasswordAsync_returns(MockAuthenticatedUserResult);
                MockConfig.With(x => x.UseEmbeddedWebView, true);

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Authenticated));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ResetPassword));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(AccessToken));
                Assert.That(result.User.UserId, Is.EqualTo(UserId));
                MockAuthenticatedAccessToken.Mock.Verify(x => x.SetAccessToken("Bearer", AccessToken));
                MockMsalClientProxy.Mock.Verify(x => x.LoginAsync(It.IsAny<object>(), true, It.IsAny<CancellationToken>()));
            }

            [Test]
            public async Task IF_authentication_requries_password_reset_and_it_is_cancelled_SHOULD_return_cancelled()
            {
                //Arrange
                MockMsalClientProxy.Where_ResetPasswordAsync_returns(MsalClientResult.Cancelled());

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Cancelled));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
                Assert.That(result.User, Is.Null);
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ResetPassword));
            }

            [Test]
            public async Task IF_authentication_requries_password_reset_and_it_fails_SHOULD_return_fail()
            {
                //Arrange
                var fail = MsalClientResult.Failed(new MsalException("MSAL Error Code"));
                MockMsalClientProxy.Where_ResetPasswordAsync_returns(fail);

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ResetPassword));
                Assert.That(result.User, Is.Null);
                Assert.That(result.ErrorMessage, Is.EqualTo($"MSAL {AuthenticationMode.ResetPassword} failed. Error code: MSAL Error Code"));
            }

            [Test]
            public async Task IF_reset_password_throws_HttpRequestException_SHOULD_return_failed_state()
            {
                //Arrange
                var exception = new HttpRequestException("Network issue");
                MockMsalClientProxy.Where_ResetPasswordAsync_throws(exception);

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
                Assert.That(result.User, Is.Null);
                Assert.That(result.ErrorMessage!.StartsWith("MSAL ResetPassword failed. Networking error"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ResetPassword));
            }

            [Test]
            public async Task IF_reset_password_throws_weird_android_network_error_SHOULD_return_failed_state()
            {
                //Arrange
                var exception = new Exception("Unable to resolve host \"minegameauth.b2clogin.com\": No address associated with hostname");
                MockMsalClientProxy.Where_ResetPasswordAsync_throws(exception);

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
                Assert.That(result.User, Is.Null);
                Assert.That(result.ErrorMessage!.StartsWith("MSAL ResetPassword failed. Networking error"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ResetPassword));

            }
        }
    }
}