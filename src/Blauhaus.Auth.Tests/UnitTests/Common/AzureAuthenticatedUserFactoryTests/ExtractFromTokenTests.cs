using System;
using Blauhaus.Analytics.TestHelpers.Extensions;
using Blauhaus.Auth.Abstractions.Errors;
using Blauhaus.Auth.Common.UserFactory;
using Blauhaus.Auth.Tests.UnitTests.Base;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Common.AzureAuthenticatedUserFactoryTests
{
    public class ExtractFromTokenTests : BaseAuthTest<AuthenticatedUserFactory>
    {
        private string _token = null!;

        public class AllTokens : ExtractFromTokenTests
        {
            
            [Test]
            public void IF_token_is_invalid_SHOULD_fail()
            {
                //Act
                var result = Sut.ExtractFromJwtToken("blooper");

                //Assert 
                MockLogger.VerifyLogErrorResponse(AuthError.InvalidToken, result);
            }
        }
        public class AzureToken : ExtractFromTokenTests
        {
            
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
         
        }
        
        public class OpenIddictToken : ExtractFromTokenTests
        {
            
        public override void Setup()
        {
            base.Setup();

            _token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IlZFQVdRQTJBRFlJMUVQSjlSMUItNU5MM1gzTktBQ0xWVUNLWUFaOU0iLCJ0eXAiOiJhdCtqd3QiLCJjdHkiOiJKV1QifQ.eyJzdWIiOiJkYmYwZGJhMC0yNTI0LTQyZjctYWFmOC1mNTg3ZWQ3NWI2NWEiLCIwM2M1YTZlNC1mNjk0LTQ4YWItOTkyMC03OTk3ZjY3MDI5ZjIiOiJkYmYwZGJhMC0yNTI0LTQyZjctYWFmOC1mNTg3ZWQ3NWI2NWEiLCJvaV9wcnN0IjoiZTAxOWZjNTItMTM2Mi00MGM0LWJmNDctNmIwOGU1MzllN2QyIiwiY2xpZW50X2lkIjoiZTAxOWZjNTItMTM2Mi00MGM0LWJmNDctNmIwOGU1MzllN2QyIiwib2lfdGtuX2lkIjoiODYyY2ZjNGYtODllNC00ZTg0LWJjOTQtYTllNzU1MjJmNzBmIiwic2NvcGUiOiJvcGVuaWQiLCJleHAiOjE2NTM5MTcxMjMsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjYzMDEvIiwiaWF0IjoxNjUzOTEzNTIzfQ.V-Wj6yi86M4RpXBLnagXz7D11EtnyGmN5cF-WoIeBOS4OaPyGLLXlwBV5vPclquHyN4UgXITUKrADZ7pRuaRcaIEumZKqi8Uc_foE_S6Cbf1SXkmgHtKDLKc4x0WpU43QSYYoaAnmO91BtVRuPSmARlHLrGr3Nozo_yn4ax4-Ic-bZDF3KG5qjjTL7owL9BjK3McfbgI9sqBxaBdOuQMSE3BMAd6Y6YA-xRaE8aJFuGZBzdFi3e-x1oxIGrvr66KE5gl-FZKOeWJ7qH8YBF-nN_y4C7p3_gNuT5_Apeam1PW_F85QPADtNHMsQ2F6X8D8vFv7HLvwE_TyxwU8eMV5w";
        }
        [Test]
        public void SHOULD_extract_Scopes()        
        { 
            //Act
            var result = Sut.ExtractFromJwtToken(_token);

            //Act
            Assert.That(result.Value.Scopes[0], Is.EqualTo("openid")); 
        }
         
        [Test]
        public void SHOULD_extract_UserId_from_Sub()       
        {
            //Act
            var result = Sut.ExtractFromJwtToken(_token);

            //Act
            Assert.That(result.Value.UserId, Is.EqualTo(Guid.Parse("dbf0dba0-2524-42f7-aaf8-f587ed75b65a")));
        }
          
        
        [Test]
        public void SHOULD_extract_Guid_properties()
        {
            //Act
            var result = Sut.ExtractFromJwtToken(_token);

            //Assert
            Assert.That(result.Value.Properties["03c5a6e4-f694-48ab-9920-7997f67029f2"], Is.EqualTo("dbf0dba0-2524-42f7-aaf8-f587ed75b65a"));
        }
         
        }

    }
}