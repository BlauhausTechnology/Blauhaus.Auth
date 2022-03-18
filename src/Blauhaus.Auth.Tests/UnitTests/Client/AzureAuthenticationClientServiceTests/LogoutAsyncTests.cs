using System;
using System.Threading.Tasks;
using Blauhaus.Auth.Tests.UnitTests.Client.AzureAuthenticationClientServiceTests.Base;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Client.AzureAuthenticationClientServiceTests
{
    public class LogoutAsyncTests : BaseAuthenticationClientServiceTest
    {
        [Test]
        public async Task SHOULD_log_out_using_MSAL_and_clear_UserIds()
        {
            //Act
            await Sut.LogoutAsync();

            //Assert
            MockMsalClientProxy.Mock.Verify(x => x.LogoutAsync());
            MockAuthenticatedAccessToken.Mock.Verify(x => x.Clear());
            MockLogger.VerifySetValue("UserId", Guid.Empty);
        }

        
    }
}