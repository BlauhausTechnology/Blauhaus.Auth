using Blauhaus.Auth.Tests.UnitTests.Common.AuthenticatedUserTests.Base;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Common.AuthenticatedUserTests
{
    public class GetClaimValuesByPrefixTests : BaseAuthenticatedUserTest
    {

        [Test]
        public void IF_user_has_matching_claims_SHOULD_return_without_prefix()
        {
            //Act
            var result = Sut.GetClaimValuesByPrefix("myPrefix_");
            
            //Assert
            Assert.That(result["Food"], Is.EqualTo("Bananas Apples Pears"));
            Assert.That(result["Dogs"], Is.EqualTo("Puppies Squirrels"));
        }
 
        [Test]
        public void IF_user_has_no_matching_claims_SHOULD_return_empty()
        {
            //Act
            var result = Sut.GetClaimValuesByPrefix("no");
            
            //Assert
            Assert.That(result.Count, Is.EqualTo(0));
        }

    }
}