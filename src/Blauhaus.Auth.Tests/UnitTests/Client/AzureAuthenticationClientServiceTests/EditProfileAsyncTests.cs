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
    public class EditProfileAsyncTests : BaseAuthenticationClientServiceTest
    {
        
        [Test]
        public async Task SHOULD_attempt_edit_profile()
        {
            //Arrange
            MockMsalClientProxy.Where_EditProfileAsync_returns(MockAuthenticatedUserResult);

            //Act
            await Sut.EditProfileAsync(MockCancelToken);

            //Assert
            MockMsalClientProxy.Mock.Verify(x => x.EditProfileAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task IF_edit_profile_succeeds_SHOULD_return_user()
        {
            //Arrange
            MockMsalClientProxy.Where_EditProfileAsync_returns(MockAuthenticatedUserResult);

            //Act
            var result = await Sut.EditProfileAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Authenticated));
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.EditProfile));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(AccessToken));
            Assert.That(result.User.UserId, Is.EqualTo(UserId));
            var userType = result.User.Claims.FirstOrDefault(x => x.Name == "UserType");
            Assert.That(userType, Is.Not.Null);
            Assert.That(userType.Value, Is.EqualTo("Admin")); 
        }

        [Test]
        public async Task IF_edit_profile_is_cancelled_SHOULD_return_cancelled()
        {
            //Arrange
            MockMsalClientProxy.Where_EditProfileAsync_returns(MsalClientResult.Cancelled());

            //Act
            var result = await Sut.EditProfileAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Cancelled));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
            Assert.That(result.User, Is.Null);
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.EditProfile)); 
        }

        [Test]
        public async Task IF_edit_profile_fails_SHOULD_return_failed_state()
        {
            //Arrange
            var fail = MsalClientResult.Failed(new MsalException("MSAL Error Code"));
            MockMsalClientProxy.Where_EditProfileAsync_returns(fail);

            //Act
            var result = await Sut.EditProfileAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
            Assert.That(result.User, Is.Null);
            Assert.That(result.ErrorMessage, Is.EqualTo($"MSAL {AuthenticationMode.EditProfile} failed. Error code: MSAL Error Code"));
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.EditProfile)); 
        }

        [Test]
        public async Task IF_edit_profile_throws_HttpRequestException_SHOULD_return_failed_state()
        {
            //Arrange
            var exception = new HttpRequestException("Network issue");
            MockMsalClientProxy.Where_EditProfileAsync_throws(exception);

            //Act
            var result = await Sut.EditProfileAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
            Assert.That(result.User, Is.Null);
            Assert.That(result.ErrorMessage, Is.EqualTo("MSAL EditProfile failed. Networking error (Network issue)"));
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.EditProfile));
        }

        [Test]
        public async Task IF_user_requries_login_SHOUD_return_fail()
        {
            //Arrange
            MockMsalClientProxy
                .Where_EditProfileAsync_returns(MsalClientResult
                    .RequiresLogin(UiRequiredExceptionClassification.ConsentRequired));

            //Act
            var result = await Sut.EditProfileAsync(MockCancelToken);

            //Assert
            Assert.That(result.AuthenticationState, Is.EqualTo(UserAuthenticationState.Failed));
            Assert.That(result.AuthenticationMode, Is.EqualTo(AuthenticationMode.EditProfile));
            Assert.That(result.ErrorMessage, Is.EqualTo("MSAL EditProfile failed. Login required"));
            Assert.That(result.AuthenticatedAccessToken, Is.EqualTo(""));
            Assert.That(result.User, Is.Null);
        }


    }
}