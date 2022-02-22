using System;
using Blauhaus.Analytics.TestHelpers.Extensions;
using Blauhaus.Auth.Abstractions.Errors;
using Blauhaus.Auth.Common;
using Blauhaus.Auth.Common.UserFactory;
using Blauhaus.Auth.Tests.UnitTests.Base;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Common.AzureAuthenticatedUserFactoryTests
{
    public class ExtractFromTokenTests : BaseAuthTest<AuthenticatedUserFactory>
    {
        private string _token;

        public override void Setup()
        {
            base.Setup();

            _token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ilg1ZVhrNHh5b2pORnVtMWtsMll0djhkbE5QNC1jNTdkTzZRR1RWQndhTmsifQ.eyJpc3MiOiJodHRwczovL21vb25iYXNlYXV0aC5iMmNsb2dpbi5jb20vZjM3MzRjMjMtMThkYy00MzE1LWFiZTMtZTIzYTQxNTA1NzJjL3YyLjAvIiwiZXhwIjoxNjAzNDY3NDE4LCJuYmYiOjE2MDM0NjM4MTgsImF1ZCI6IjJiYmVmZjMxLTZmY2YtNDg4ZC04NmQ2LTU1NGVjODk4MjhjMiIsImlkcCI6IkxvY2FsQWNjb3VudCIsIm9pZCI6IjIzODMzOTY1LTkwYzEtNGRiOC04YWFmLWVlYTAzYTg0NDAzMiIsInN1YiI6IjIzODMzOTY1LTkwYzEtNGRiOC04YWFmLWVlYTAzYTg0NDAzMiIsImVtYWlscyI6WyJ0ZXN0ZXIxQGJsYXVoYXVzdGVjaG5vbG9neS5jb20iXSwidGZwIjoiQjJDXzFfTW9vbmJhc2VfUk9QQyIsInNjcCI6IlJlYWQuQW5kLldyaXRlIiwiYXpwIjoiMmJiZWZmMzEtNmZjZi00ODhkLTg2ZDYtNTU0ZWM4OTgyOGMyIiwidmVyIjoiMS4wIiwiaWF0IjoxNjAzNDYzODE4fQ.M3JYgsQXqWBPzeI3E-2ALUHB0nxDsFjlbGerbRV9J_4plaPxkhHab7cy-ZhG4o9ELm-iT6DYh9re003fClNDzcg27_RS2loMhsVz3uS47EfThpB0VkSyGOml8oLlPOQLDyGysmSFhhuPuapv3bxcKGsN_IBCZdmMaQgwEY7AMq5pPzrF2jM0y7TzMkrhN6Lr8OV7Bz-K3PXfBjks79r2rhKtclzTu0t8BMQ_sMzjPcyRAovOuufISKoPQg6EVoHnaT9Up01iUKQC5fxXL10RMTpwnQ6975bSrQXjzoDc8L1TrfPSgy-hQztCcIPERUxPFRbDxl6uObIo_6ICvkyRYw";
        }

        [Test]
        public void SHOULD_extract_Policy()        
        {
            //Act
            var result = Sut.ExtractFromJwtToken(_token);

            //Act
            Assert.That(result.Value.AuthPolicy, Is.EqualTo("B2C_1_Moonbase_ROPC"));
        }

        [Test]
        public void SHOULD_extract_Scopes()        
        { 
            //Act
            var result = Sut.ExtractFromJwtToken(_token);

            //Act
            Assert.That(result.Value.Scopes[0], Is.EqualTo("Read.And.Write")); 
        }
         
        [Test]
        public void SHOULD_extract_UserId_from_Sub()       
        {
            //Act
            var result = Sut.ExtractFromJwtToken(_token);

            //Act
            Assert.That(result.Value.UserId, Is.EqualTo(Guid.Parse("23833965-90c1-4db8-8aaf-eea03a844032")));
        }
          
        
        [Test]
        public void IF_ClaimsPrincipal_has_email_address_SHOULD_add()
        {
            //Act
            var result = Sut.ExtractFromJwtToken(_token);

            //Assert
            Assert.That(result.Value.EmailAddress, Is.EqualTo("tester1@blauhaustechnology.com"));
        }
         
        
        [Test]
        public void IF_token_is_invalid_SHOULD_fail()
        {
            //Act
            var result = Sut.ExtractFromJwtToken("blooper");

            //Assert
            result.VerifyResponseError(AuthError.InvalidToken, MockAnalyticsService);
        }

    }
}