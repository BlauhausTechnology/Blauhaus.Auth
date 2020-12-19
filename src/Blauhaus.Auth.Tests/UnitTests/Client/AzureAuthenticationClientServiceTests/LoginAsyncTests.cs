using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Models;
using Blauhaus.Auth.Client.Azure.Config;
using Blauhaus.Auth.Client.Azure.MsalProxy;
using Blauhaus.Auth.Tests.UnitTests.Client.AzureAuthenticationClientServiceTests._Base;
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
                Assert.That(result.User.UserId, Is.EqualTo(UserId));
                Assert.That(result.User.EmailAddress, Is.EqualTo("adrian@maxxor.com"));
                var userType = result.User.Claims.FirstOrDefault(x => x.Name == "UserType");
                Assert.That(userType, Is.Not.Null);
                Assert.That(userType.Value, Is.EqualTo("Admin"));
                MockAuthenticatedAccessToken.Mock.Verify(x => x.SetAccessToken("Bearer", AccessToken));
                MockAnalyticsService.MockCurrentSession.Mock.VerifySet(x => x.UserId = UserId.ToString());
                MockAnalyticsService.Mock.Verify(x => x.Trace(Sut, "SilentLogin successful", LogSeverity.Information, 
                    It.Is<Dictionary<string, object>>(y => (Guid) y["UserId"] == UserId), It.IsAny<string>()));
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
                var userType = result.User.Claims.FirstOrDefault(x => x.Name == "UserType");
                Assert.That(userType, Is.Not.Null);
                Assert.That(userType.Value, Is.EqualTo("Admin"));
                MockAuthenticatedAccessToken.Mock.Verify(x => x.SetAccessToken("Bearer", AccessToken));
                MockAnalyticsService.Mock.Verify(x => x.Trace(Sut, "SilentLogin successful", LogSeverity.Information, 
                    It.Is<Dictionary<string, object>>(y => (Guid) y["UserId"] == UserId), It.IsAny<string>()));
                MockAnalyticsService.Mock.Verify(x => x.Trace(Sut, "SilentLogin successful", LogSeverity.Information, 
                    It.Is<Dictionary<string, object>>(y => ((List<string>) y["MsalLogs"]).Count == 1), It.IsAny<string>()));
            }


            [Test]
            public async Task IF_silent_authentication_is_cancelled_SHOULD_return_cancelled()
            {
                //Arrange
                MockMsalClientProxy.Where_AuthenticateSilentlyAsync_returns(MsalClientResult.Cancelled(MockLogs));

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Cancelled));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
                Assert.That(result.User, Is.Null);
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.SilentLogin));
                MockAnalyticsService.Mock.Verify(x => x.Trace(Sut, "SilentLogin cancelled. MSAL state: Cancelled",
                    LogSeverity.Information, It.IsAny<Dictionary<string, object>>(), It.IsAny<string>()));
            }

            [Test]
            public async Task IF_silent_authentication_fails_SHOULD_return_failed_state()
            {
                //Arrange
                var fail = MsalClientResult.Failed(new MsalException("MSAL Error Code"), MockLogs);
                MockMsalClientProxy.Where_AuthenticateSilentlyAsync_returns(fail);

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
                Assert.That(result.User, Is.Null);
                Assert.That(result.ErrorMessage, Is.EqualTo($"MSAL {AuthenticationMode.SilentLogin} failed. Error code: MSAL Error Code"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.SilentLogin));
                MockAnalyticsService.VerifyTrace("SilentLogin FAILED: MSAL Error Code. MSAL state: Failed", LogSeverity.Warning);
                MockAnalyticsService.VerifyTraceProperty(y => (MsalClientResult)y["MSAL result"] == fail);
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
                Assert.That(result.ErrorMessage, Is.EqualTo("MSAL SilentLogin failed. Networking error"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.SilentLogin));
                MockAnalyticsService.VerifyLogException(exception);
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
                Assert.That(result.ErrorMessage, Is.EqualTo("MSAL SilentLogin failed. Networking error"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.SilentLogin));
                MockAnalyticsService.VerifyLogException(e);

            }

        }

        public class ManualLogin : LoginAsyncTests
        {
            public override void Setup()
            {
                base.Setup();

                MockMsalClientProxy
                    .Where_AuthenticateSilentlyAsync_returns(MsalClientResult
                        .RequiresLogin(UiRequiredExceptionClassification.ConsentRequired, MockLogs));
            }

            [Test]
            public async Task IF_authentication_requries_login_SHOULD_log_reason()
            {
                //Arrange
                MockMsalClientProxy.Where_LoginAsync_returns(MockAuthenticatedUserResult);

                //Act
                await Sut.LoginAsync(MockCancelToken);

                //Assert
                MockAnalyticsService.Mock.Verify(x => x.Trace(Sut, "Manual Login Required because ConsentRequired", LogSeverity.Information, 
                    It.Is<Dictionary<string, object>>(y => ((List<string>) y["MsalLogs"]).Count == 1), It.IsAny<string>()));
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
                MockAnalyticsService.VerifyTrace("ManualLogin successful", LogSeverity.Information);
                MockAnalyticsService.VerifyTraceProperty("UserId", UserId);
                MockAnalyticsService.MockCurrentSession.Mock.VerifySet(x => x.UserId = UserId.ToString());
                MockMsalClientProxy.Mock.Verify(x => x.LoginAsync(It.IsAny<object>(), true, It.IsAny<CancellationToken>()));
            }

            [Test]
            public async Task IF_authentication_requries_login_and_login_is_cancelled_SHOULD_return_cancellation()
            {
                //Arrange
                MockMsalClientProxy.Where_LoginAsync_returns(MsalClientResult.Cancelled(MockLogs));

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Cancelled));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ManualLogin));
                Assert.That(result.User, Is.Null);
                MockAnalyticsService.VerifyTrace("ManualLogin cancelled. MSAL state: Cancelled", LogSeverity.Information);
            }

            [Test]
            public async Task IF_authentication_requries_login_and_login_fails_SHOULD_return_Failed()
            {
                //Arrange
                var fail = MsalClientResult.Failed(new MsalException("MSAL Error Code"), MockLogs);
                MockMsalClientProxy.Where_LoginAsync_returns(fail);

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
                Assert.That(result.User, Is.Null);
                Assert.That(result.ErrorMessage, Is.EqualTo($"MSAL {AuthenticationMode.ManualLogin} failed. Error code: MSAL Error Code"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ManualLogin));
                MockAnalyticsService.VerifyTrace("ManualLogin FAILED: MSAL Error Code. MSAL state: Failed", LogSeverity.Warning);
                MockAnalyticsService.VerifyTraceProperty("MSAL result", fail);
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
                Assert.That(result.ErrorMessage, Is.EqualTo("MSAL ManualLogin failed. Networking error"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ManualLogin));
                MockAnalyticsService.VerifyLogException(exception);
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
                Assert.That(result.ErrorMessage, Is.EqualTo("MSAL ManualLogin failed. Networking error"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ManualLogin));
                MockAnalyticsService.VerifyLogException(exception);
            }
        }

        public class ResetPassword : LoginAsyncTests
        {
            
            public override void Setup()
            {
                base.Setup();

                MockMsalClientProxy
                    .Where_AuthenticateSilentlyAsync_returns(MsalClientResult
                        .RequiresLogin(UiRequiredExceptionClassification.ConsentRequired, MockLogs))
                    .Where_LoginAsync_returns(MsalClientResult.RequiresPasswordReset(MockLogs));
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
                MockAnalyticsService.VerifyTrace("ResetPassword successful", LogSeverity.Information);
                MockAnalyticsService.VerifyTraceProperty("UserId", UserId);
                MockAnalyticsService.Mock.Verify(x => x.Trace(Sut, It.IsAny<string>(), LogSeverity.Information, 
                    It.Is<Dictionary<string, object>>(y => ((List<string>) y["MsalLogs"]).Count == 1), It.IsAny<string>()));
                MockMsalClientProxy.Mock.Verify(x => x.LoginAsync(It.IsAny<object>(), true, It.IsAny<CancellationToken>()));
            }

            [Test]
            public async Task IF_authentication_requries_password_reset_and_it_is_cancelled_SHOULD_return_cancelled()
            {
                //Arrange
                MockMsalClientProxy.Where_ResetPasswordAsync_returns(MsalClientResult.Cancelled(MockLogs));

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Cancelled));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
                Assert.That(result.User, Is.Null);
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ResetPassword));
                MockAnalyticsService.VerifyTrace("ResetPassword cancelled. MSAL state: Cancelled", LogSeverity.Information);
            }

            [Test]
            public async Task IF_authentication_requries_password_reset_and_it_fails_SHOULD_return_fail()
            {
                //Arrange
                var fail = MsalClientResult.Failed(new MsalException("MSAL Error Code"), MockLogs);
                MockMsalClientProxy.Where_ResetPasswordAsync_returns(fail);

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ResetPassword));
                Assert.That(result.User, Is.Null);
                Assert.That(result.ErrorMessage, Is.EqualTo($"MSAL {AuthenticationMode.ResetPassword} failed. Error code: MSAL Error Code"));
                MockAnalyticsService.VerifyTrace("ResetPassword FAILED: MSAL Error Code. MSAL state: Failed", LogSeverity.Warning);
                MockAnalyticsService.VerifyTraceProperty("MSAL result", fail);
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
                Assert.That(result.ErrorMessage, Is.EqualTo("MSAL ResetPassword failed. Networking error"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ResetPassword));
                MockAnalyticsService.VerifyLogException(exception);
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
                Assert.That(result.ErrorMessage, Is.EqualTo("MSAL ResetPassword failed. Networking error"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ResetPassword));
                MockAnalyticsService.VerifyLogException(exception);

            }
        }
    }
}