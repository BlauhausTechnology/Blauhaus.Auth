﻿using System;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Builders;
using Blauhaus.Auth.Tests.UnitTests.Server.AzureAuthenticationServerServiceTests._Base;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Server.AzureAuthenticationServerServiceTests
{
    public class CreateTests : BaseAzureAuthenticationServerServiceTest
    {

        private readonly Guid _userId = Guid.NewGuid();
        
        [Test]
        public void SHOULD_extract_UserId_from_ObjectIdentifier()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_UserObjectId(_userId).Build();

            //Act
            var result = Sut.ExtractUserFromClaimsPrincipal(claimsPrincipal);

            //Act
            Assert.That(result.UserId, Is.EqualTo(_userId));
        }
        
        [Test]
        public void IF_ClaimsPrincipal_does_not_have_ObjectIdentifier_SHOULD_throw_exception_and_log()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder().Build();

            //Assert
            Assert.Throws<UnauthorizedAccessException>(() => Sut.ExtractUserFromClaimsPrincipal(claimsPrincipal), "Invalid Identity");

        }
        
        [Test]
        public void IF_ClaimsPrincipal_has_empty_ObjectIdentifier_SHOULD_throw_exception()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_UserObjectId(Guid.Empty).Build();

            //Assert
            Assert.Throws<UnauthorizedAccessException>(() => Sut.ExtractUserFromClaimsPrincipal(claimsPrincipal), "Invalid Identity");
        }

        [Test]
        public void IF_ClaimsPrincipal_is_not_authenticated_SHOULD_throw_exception_and_log()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_UserObjectId(_userId)
                .WithIsAuthenticatedFalse().Build();

            //Assert
            Assert.Throws<UnauthorizedAccessException>(() => Sut.ExtractUserFromClaimsPrincipal(claimsPrincipal), "User is not authenticated");
        }
        
        [Test]
        public void IF_ClaimsPrincipal_has_email_address_SHOULD_add()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_Claim("emails", "bob@freever.com")
                .With_UserObjectId(_userId).Build();

            //Act
            var result = Sut.ExtractUserFromClaimsPrincipal(claimsPrincipal);

            //Assert
            Assert.That(result.EmailAddress, Is.EqualTo("bob@freever.com"));
        }

        [Test]
        public void IF_ClaimsPrincipal_does_not_have_email_address_SHOULD_set_to_null()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_UserObjectId(_userId).Build();

            //Act
            var result = Sut.ExtractUserFromClaimsPrincipal(claimsPrincipal);

            //Assert
            Assert.That(result.EmailAddress, Is.Null);
        }

        [Test]
        public void IF_ClaimsPrincipal_has_empty_email_address_SHOULD_set_to_null()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_UserObjectId(_userId)
                .With_Claim("emails", "").Build();

            //Act
            var result = Sut.ExtractUserFromClaimsPrincipal(claimsPrincipal);

            //Assert
            Assert.That(result.EmailAddress, Is.Null);
        }
        
        [Test]
        public void IF_ClaimsPrincipal_has_azure_claims_SHOULD_extract_them()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder()
                .With_UserObjectId(_userId)
                .With_Claim("extension_HappyId", "12345").Build();

            //Act
            var result = Sut.ExtractUserFromClaimsPrincipal(claimsPrincipal);

            //Assert
            Assert.That(result.HasClaimValue("HappyId", "12345"), Is.True);
        } 

    }
}