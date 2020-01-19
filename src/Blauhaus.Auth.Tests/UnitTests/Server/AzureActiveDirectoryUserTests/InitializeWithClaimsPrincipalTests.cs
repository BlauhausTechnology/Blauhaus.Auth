using System;
using Blauhaus.Auth.Abstractions.Builders;
using Blauhaus.Auth.Server.Azure.User;
using Blauhaus.Common.TestHelpers;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Server.AzureActiveDirectoryUserTests
{
    public class InitializeWithClaimsPrincipalTests : BaseUnitTest<DefaultAzureActiveDirectoryUser>
    {
        protected override DefaultAzureActiveDirectoryUser ConstructSut()
        {
            return new DefaultAzureActiveDirectoryUser();
        }

        [SetUp]
        public void Setup()
        {
            Cleanup();
        }

        [Test]
        public void SHOULD_extract_AuthenticatedUserId_from_ObjectIdentifier()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_ObjectId("objectId").Build();

            //Act
            Sut.Initialize(claimsPrincipal);

            //Act
            Assert.That(Sut.AuthenticatedUserId, Is.EqualTo("objectId"));
        }
        
        [Test]
        public void IF_ClaimsPrincipal_does_not_have_ObjectIdentifier_SHOULD_throw_exception()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder().Build();

            //Assert
            Assert.Throws<UnauthorizedAccessException>(() => Sut.Initialize(claimsPrincipal), "Invalid identity");
        }
        
        [Test]
        public void IF_ClaimsPrincipal_has_empty_ObjectIdentifier_SHOULD_throw_exception()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_ObjectId("").Build();

            //Assert
            Assert.Throws<UnauthorizedAccessException>(() => Sut.Initialize(claimsPrincipal), "Invalid identity");
        }

        [Test]
        public void IF_ClaimsPrincipal_is_not_authenticated_SHOULD_throw_exception()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_ObjectId("objectId")
                .WithIsAuthenticatedFalse().Build();

            //Assert
            Assert.Throws<UnauthorizedAccessException>(() => Sut.Initialize(claimsPrincipal), "User is not authenticated");
        }
        
        [Test]
        public void IF_ClaimsPrincipal_has_email_address_SHOULD_add()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_Claim("emails", "bob@freever.com")
                .With_ObjectId("objectId").Build();

            //Act
            Sut.Initialize(claimsPrincipal);

            //Assert
            Assert.That(Sut.EmailAddress, Is.EqualTo("bob@freever.com"));
        }

        [Test]
        public void IF_ClaimsPrincipal_does_not_have_email_address_SHOULD_set_to_null()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_ObjectId("objectId").Build();

            //Act
            Sut.Initialize(claimsPrincipal);

            //Assert
            Assert.That(Sut.EmailAddress, Is.Null);
        }

        [Test]
        public void IF_ClaimsPrincipal_has_empty_email_address_SHOULD_set_to_null()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_ObjectId("objectId")
                .With_Claim("emails", "").Build();

            //Act
            Sut.Initialize(claimsPrincipal);

            //Assert
            Assert.That(Sut.EmailAddress, Is.Null);
        }

    }
}