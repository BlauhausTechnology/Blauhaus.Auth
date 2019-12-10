using System.Threading.Tasks;
using Blauhaus.Auth.Tests.UnitTests.Client.AzureAuthenticationClientServiceTests._Base;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Client.AzureAuthenticationClientServiceTests
{
    public class LogoutAsyncTests : BaseAuthenticationClientServiceTest
    {
        [Test]
        public async Task SHOULD_log_out_using_MSAL()
        {
            //Act
            await Sut.LogoutAsync();

            //Assert
            MockMsalClientProxy.Mock.Verify(x => x.LogoutAsync());
            MockAuthenticatedAccessToken.Mock.Verify(x => x.Clear());
        }

        
    }
}