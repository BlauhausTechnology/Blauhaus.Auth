using Blauhaus.Auth.Tests.UnitTests.Common.AuthenticatedUserTests.Base;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Common.AuthenticatedUserTests
{
    public class HasClaimValueTests : BaseAuthenticatedUserTest
    {
        [Test]
        public void IF_user_has_matching_claim_SHOULD_return_true()
        {
            Assert.That(Sut.HasClaimValue("FavouriteColour", "Blue"), Is.True);
            Assert.That(Sut.HasClaimValue("FavouriteBand", "Deep Purple"), Is.True);
        }
        
        [Test]
        public void IF_user_has_matching_claim_in_different_case_SHOULD_return_true()
        {
            Assert.That(Sut.HasClaimValue("favouriteColour", "Blue"), Is.True);
            Assert.That(Sut.HasClaimValue("FavouriteBand", "deep Purple"), Is.True);
        }
        
        [Test]
        public void IF_user_has_matching_claim_with_wrong_value_SHOULD_return_false()
        {
            Assert.That(Sut.HasClaimValue("FavouriteColour", "Pink"), Is.False);
            Assert.That(Sut.HasClaimValue("FavouriteBand", "Pink Floyd"), Is.False);
        }
    
        [Test]
        public void IF_user_does_not_have_matching_claim_SHOULD_return_false()
        {
            Assert.That(Sut.HasClaimValue("BestMeme", "Anything"), Is.False);
        }
    }
}