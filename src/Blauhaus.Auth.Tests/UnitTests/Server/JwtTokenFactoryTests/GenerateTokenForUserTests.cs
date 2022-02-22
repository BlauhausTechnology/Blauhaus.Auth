using System;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Errors;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Auth.Server.Jwt.TokenFactory;
using System.IdentityModel.Tokens.Jwt;
using Blauhaus.Auth.Tests.UnitTests.Server.JwtTokenFactoryTests.Base;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Server.JwtTokenFactoryTests;

public class GenerateTokenForUserTests : BaseJwtTokenFactoryTest
{

    [Test]
    public void SHOULD_add_UserId_and_email_address()
    {
        //Arrange
        var userId = Guid.NewGuid();
        MockUser
            .With(x => x.EmailAddress, "bob@freever.com")
            .With_UserId(userId);


        //Act
        var token = Sut.GenerateTokenForUser(MockUser.Object);
        var result = AuthenticatedUserFactorty.ExtractFromJwtToken(token);

        //Assert
        Assert.That(result.Value.UserId, Is.EqualTo(userId));
        Assert.That(result.Value.EmailAddress, Is.EqualTo("bob@freever.com"));
    }
    
    [Test]
    public void SHOULD_add_Random_claims()
    {
        //Arrange
        MockUser
            .With_Claim(new UserClaim("FavouriteColour", "Blue"));

        //Act
        var token = Sut.GenerateTokenForUser(MockUser.Object);
        var result = AuthenticatedUserFactorty.ExtractFromJwtToken(token);

        //Assert
        Assert.That(result.Value.HasClaimValue("FavouriteColour", "Blue"));
    }

    [Test]
    public void SHOULD_add_ExpiresAt_and_Nbf()
    {
        //Act
        var token = Sut.GenerateTokenForUser(MockUser.Object);
        var result = new JwtSecurityTokenHandler().ReadJwtToken(token);
        
        //Assert
        Assert.That(result.ValidFrom, Is.EqualTo(MockTimeService.Object.CurrentUtcTime));
        Assert.That(result.ValidTo, Is.EqualTo(MockTimeService.Object.CurrentUtcTime.Add(MockJwtTokenConfig.Object.ValidFor)));
    }
		
}