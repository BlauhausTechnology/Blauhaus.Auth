using System;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Builders;
using Blauhaus.Auth.Abstractions.Errors;
using Blauhaus.Auth.Server.Azure.Service;
using Blauhaus.Auth.Tests.UnitTests._Base;
using Blauhaus.Errors.Extensions;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Server.AzureAuthenticatedUserFactoryTests
{
    public class CreateTests : BaseAuthTest<AzureAuthenticatedUserFactory>
    {
        private readonly Guid _userId = Guid.NewGuid();

    
        [Test]
        public void SHOULD_extract_UserId_from_ObjectIdentifier()        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_UserObjectId(_userId).Build();

            //Act
            var result = Sut.Create(claimsPrincipal);

            //Act
            Assert.That(result.Value.UserId, Is.EqualTo(_userId));
        }
        
        [Test]
        public void IF_ClaimsPrincipal_does_not_have_ObjectIdentifier_SHOULD_throw_exception_and_log()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder().Build();

            //Act
            var result = Sut.Create(claimsPrincipal);

            //Assert
            Assert.That(result.Error.ToError(), Is.EqualTo(AuthErrors.InvalidIdentity)); 
            MockAnalyticsService.VerifyTrace(AuthErrors.InvalidIdentity.Code, LogSeverity.Error);

        }
        
        [Test]
        public void IF_ClaimsPrincipal_has_empty_ObjectIdentifier_SHOULD_throw_exception()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_UserObjectId(Guid.Empty).Build();
            //Act
            var result = Sut.Create(claimsPrincipal);

            //Assert
            Assert.That(result.Error.ToError(), Is.EqualTo(AuthErrors.InvalidIdentity)); 
            MockAnalyticsService.VerifyTrace(AuthErrors.InvalidIdentity.Code, LogSeverity.Error);
        }

        [Test]
        public void IF_ClaimsPrincipal_is_not_authenticated_SHOULD_throw_exception_and_log()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_UserObjectId(_userId)
                .WithIsAuthenticatedFalse().Build();
            //Act
            var result = Sut.Create(claimsPrincipal);

            //Assert
            Assert.That(result.Error.ToError(), Is.EqualTo(AuthErrors.NotAuthenticated)); 
            MockAnalyticsService.VerifyTrace(AuthErrors.NotAuthenticated.Code, LogSeverity.Error);
        }
        
        [Test]
        public void IF_ClaimsPrincipal_has_email_address_SHOULD_add()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_Claim("emails", "bob@freever.com")
                .With_UserObjectId(_userId).Build();

            //Act
            var result = Sut.Create(claimsPrincipal);

            //Assert
            Assert.That(result.Value.EmailAddress, Is.EqualTo("bob@freever.com"));
        }

        [Test]
        public void IF_ClaimsPrincipal_does_not_have_email_address_SHOULD_set_to_null()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_UserObjectId(_userId).Build();

            //Act
            var result = Sut.Create(claimsPrincipal);

            //Assert
            Assert.That(result.Value.EmailAddress, Is.Null);
        }

        [Test]
        public void IF_ClaimsPrincipal_has_empty_email_address_SHOULD_set_to_null()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_UserObjectId(_userId)
                .With_Claim("emails", "").Build();

            //Act
            var result = Sut.Create(claimsPrincipal);

            //Assert
            Assert.That(result.Value.EmailAddress, Is.Null);
        }
        
        [Test]
        public void IF_ClaimsPrincipal_has_azure_claims_SHOULD_extract_them()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_UserObjectId(_userId)
                .With_Claim("extension_HappyId", "12345").Build();

            //Act
            var result = Sut.Create(claimsPrincipal);

            //Assert
            Assert.That(result.Value.HasClaimValue("HappyId", "12345"), Is.True);
        }

        [Test]
        public void SHOULD_trace()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_UserObjectId(_userId)
                .With_Claim("emails", "bob@freever.com")
                .With_NameIdentifier("MyNameIs").Build();

            //Act
            Sut.Create(claimsPrincipal);

            //Assert
            MockAnalyticsService.VerifyTrace("User profile extracted from ClaimsPrincipal: " + _userId);

        }

    }
}