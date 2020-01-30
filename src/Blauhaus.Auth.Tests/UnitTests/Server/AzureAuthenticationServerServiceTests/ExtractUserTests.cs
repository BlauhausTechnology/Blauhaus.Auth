using Blauhaus.Auth.Abstractions.Builders;
using Blauhaus.Auth.Server.Azure.User;
using Blauhaus.Auth.Tests.UnitTests.Server.AzureAuthenticationServerServiceTests._Base;
using Blauhaus.Common.TestHelpers;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Server.AzureAuthenticationServerServiceTests
{
    public class ExtractUserTests : BaseAzureAuthenticationServerServiceTest
    {

        [Test]
        public void SHOULD_initialize_User_with_ClaimsPrincipal_and_return()
        {
            //Arrange
            var mockUser = new MockBuilder<IAzureActiveDirectoryUser>();
            MockIocService.Mock.Setup(x => x.Resolve<IAzureActiveDirectoryUser>()).Returns(mockUser.Object);
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_NameIdentifier("MyNameIs").Build();

            //Act
            var result = Sut.ExtractUser(claimsPrincipal);

            //Assert
            mockUser.Mock.Verify(x => x.Initialize(claimsPrincipal));
            Assert.That(result, Is.EqualTo(mockUser.Object));
        }


    }
}