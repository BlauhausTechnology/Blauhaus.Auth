using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
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
        public class SilentLogin : LoginAsyncTests
        {
            [Test]
            public async Task IF_silent_authentication_succeeds_SHOULD_call_HandleAccessToken_and_return_user()
            {
                //Arrange
                MockMsalClientProxy.Where_AuthenticateSilentlyAsync_returns(MockAuthenticatedUserResult);

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Authenticated));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.SilentLogin));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo("authenticatedAccesstoken"));
                Assert.That(result.AuthenticatedUserId, Is.EqualTo("authenticatedUserId"));
                MockAuthenticatedAccessToken.Mock.Verify(x => x.SetAccessToken("Bearer", "authenticatedAccesstoken"));
                MockAnalyticsService.Mock.Verify(x => x.Trace(Sut, "SilentLogin successful for authenticatedUserId", LogSeverity.Information, 
                    It.Is<Dictionary<string, object>>(y => (string) y["AuthenticatedUserId"] == "authenticatedUserId"), It.IsAny<string>()));
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
                Assert.That(result.AuthenticatedUserId, Is.EqualTo(""));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.SilentLogin));
                MockAnalyticsService.Mock.Verify(x => x.Trace(Sut, "SilentLogin cancelled. MSAL state: Cancelled",
                    LogSeverity.Information, It.IsAny<Dictionary<string, object>>(), It.IsAny<string>()));
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
                Assert.That(result.AuthenticatedUserId, Is.EqualTo(""));
                Assert.That(result.ErrorMessage, Is.EqualTo($"MSAL {AuthenticationMode.SilentLogin} failed. Error code: MSAL Error Code"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.SilentLogin));
                MockAnalyticsService.Mock.Verify(x => x.Trace(Sut, "SilentLogin FAILED: MSAL Error Code. MSAL state: Failed", LogSeverity.Warning, 
                    It.Is<Dictionary<string, object>>(y => y["MSAL result"] == fail), It.IsAny<string>()));
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
                Assert.That(result.AuthenticatedUserId, Is.EqualTo(""));
                Assert.That(result.ErrorMessage, Is.EqualTo("MSAL SilentLogin failed. Networking error"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.SilentLogin));
                MockAnalyticsService.Mock.Verify(x => x.LogException(Sut, exception, 
                    It.IsAny<Dictionary<string, object>>(), It.IsAny<Dictionary<string, double>>(), It.IsAny<string>()));
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
                Assert.That(result.AuthenticatedUserId, Is.EqualTo(""));
                Assert.That(result.ErrorMessage, Is.EqualTo("MSAL SilentLogin failed. Networking error"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.SilentLogin));
                MockAnalyticsService.Mock.Verify(x => x.LogException(Sut, e, It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<Dictionary<string, double>>(), It.IsAny<string>()));

            }

        }

        public class ManualLogin : LoginAsyncTests
        {
            public override void Setup()
            {
                base.Setup();

                MockMsalClientProxy
                    .Where_AuthenticateSilentlyAsync_returns(MsalClientResult.RequiresLogin());
            }

            [Test]
            public async Task IF_authentication_requries_login_and_login_succeeds_SHOULD_call_HandleAccessToken_and_return_user()
            {
                //Arrange
                MockMsalClientProxy.Where_LoginAsync_returns(MockAuthenticatedUserResult);

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Authenticated));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ManualLogin));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo("authenticatedAccesstoken"));
                Assert.That(result.AuthenticatedUserId, Is.EqualTo("authenticatedUserId"));
                MockAuthenticatedAccessToken.Mock.Verify(x => x.SetAccessToken("Bearer", "authenticatedAccesstoken"));
                MockAnalyticsService.Mock.Verify(x => x.Trace(Sut, "ManualLogin successful for authenticatedUserId", LogSeverity.Information, 
                    It.Is<Dictionary<string, object>>(y => (string) y["AuthenticatedUserId"] == "authenticatedUserId"), It.IsAny<string>()));
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
                Assert.That(result.AuthenticatedUserId, Is.EqualTo(""));
                MockAnalyticsService.Mock.Verify(x => x.Trace(Sut, "ManualLogin cancelled. MSAL state: Cancelled", 
                    LogSeverity.Information, It.IsAny<Dictionary<string, object>>(), It.IsAny<string>()));
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
                Assert.That(result.AuthenticatedUserId, Is.EqualTo(""));
                Assert.That(result.ErrorMessage, Is.EqualTo($"MSAL {AuthenticationMode.ManualLogin} failed. Error code: MSAL Error Code"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ManualLogin));
                MockAnalyticsService.Mock.Verify(x => x.Trace(Sut, "ManualLogin FAILED: MSAL Error Code. MSAL state: Failed", LogSeverity.Warning,
                    It.Is<Dictionary<string, object>>(y => y["MSAL result"] == fail), It.IsAny<string>()));
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
                Assert.That(result.AuthenticatedUserId, Is.EqualTo(""));
                Assert.That(result.ErrorMessage, Is.EqualTo("MSAL ManualLogin failed. Networking error"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ManualLogin));
                MockAnalyticsService.Mock.Verify(x => x.LogException(Sut, exception, It.IsAny<Dictionary<string, object>>(), It.IsAny<Dictionary<string, double>>(), It.IsAny<string>()));
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
                Assert.That(result.AuthenticatedUserId, Is.EqualTo(""));
                Assert.That(result.ErrorMessage, Is.EqualTo("MSAL ManualLogin failed. Networking error"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ManualLogin));
                MockAnalyticsService.Mock.Verify(x => x.LogException(Sut, exception, It.IsAny<Dictionary<string, object>>(), It.IsAny<Dictionary<string, double>>(), It.IsAny<string>()));

            }
        }

        public class ResetPassword : LoginAsyncTests
        {
            
            public override void Setup()
            {
                base.Setup();

                MockMsalClientProxy
                    .Where_AuthenticateSilentlyAsync_returns(MsalClientResult.RequiresLogin())
                    .Where_LoginAsync_returns(MsalClientResult.RequiresPasswordReset());
            }

            [Test]
            public async Task IF_authentication_requries_password_reset_and_it_succeeds_SHOULD_call_HandleAccessToken_and_return_user()
            {
                //Arrange
                MockMsalClientProxy.Where_ResetPasswordAsync_returns(MockAuthenticatedUserResult);

                //Act
                var result = await Sut.LoginAsync(MockCancelToken);

                //Assert
                Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Authenticated));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ResetPassword));
                Assert.That(result.AuthenticatedAccessToken, Is.EqualTo("authenticatedAccesstoken"));
                Assert.That(result.AuthenticatedUserId, Is.EqualTo("authenticatedUserId"));
                MockAuthenticatedAccessToken.Mock.Verify(x => x.SetAccessToken("Bearer", "authenticatedAccesstoken"));
                MockAnalyticsService.Mock.Verify(x => x.Trace(Sut, "ResetPassword successful for authenticatedUserId", LogSeverity.Information, 
                    It.Is<Dictionary<string, object>>(y => (string) y["AuthenticatedUserId"] == "authenticatedUserId"), It.IsAny<string>()));
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
                Assert.That(result.AuthenticatedUserId, Is.EqualTo(""));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ResetPassword));
                MockAnalyticsService.Mock.Verify(x => x.Trace(Sut, "ResetPassword cancelled. MSAL state: Cancelled", 
                    LogSeverity.Information, It.IsAny<Dictionary<string, object>>(), It.IsAny<string>()));

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
                Assert.That(result.AuthenticatedUserId, Is.EqualTo(""));
                Assert.That(result.ErrorMessage, Is.EqualTo($"MSAL {AuthenticationMode.ResetPassword} failed. Error code: MSAL Error Code"));
                MockAnalyticsService.Mock.Verify(x => x.Trace(Sut, "ResetPassword FAILED: MSAL Error Code. MSAL state: Failed", LogSeverity.Warning,
                    It.Is<Dictionary<string, object>>(y => y["MSAL result"] == fail), It.IsAny<string>()));
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
                Assert.That(result.AuthenticatedUserId, Is.EqualTo(""));
                Assert.That(result.ErrorMessage, Is.EqualTo("MSAL ResetPassword failed. Networking error"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ResetPassword));
                MockAnalyticsService.Mock.Verify(x => x.LogException(Sut, exception, 
                    It.IsAny<Dictionary<string, object>>(), It.IsAny<Dictionary<string, double>>(), It.IsAny<string>()));
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
                Assert.That(result.AuthenticatedUserId, Is.EqualTo(""));
                Assert.That(result.ErrorMessage, Is.EqualTo("MSAL ResetPassword failed. Networking error"));
                Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.ResetPassword));
                MockAnalyticsService.Mock.Verify(x => x.LogException(Sut, exception, 
                    It.IsAny<Dictionary<string, object>>(), It.IsAny<Dictionary<string, double>>(), It.IsAny<string>()));

            }
        }
    }
}