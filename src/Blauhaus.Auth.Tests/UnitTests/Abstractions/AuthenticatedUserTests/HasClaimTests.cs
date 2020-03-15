using Blauhaus.Auth.Tests.UnitTests.Abstractions.AuthenticatedUserTests._Base;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Abstractions.AuthenticatedUserTests
{
    public class HasClaimTests : BaseAuthenticatedUserTest
    {

        [Test]
        public void IF_user_has_matching_claim_SHOULD_return_true()
        {
            Assert.That(Sut.HasClaim("FavouriteColour"), Is.True);
            Assert.That(Sut.HasClaim("FavouriteBand"), Is.True);
        }

        [Test]
        public void IF_user_has_matching_claim_with_different_casing_SHOULD_return_true()
        {
            Assert.That(Sut.HasClaim("favouriteColour"), Is.True);
            Assert.That(Sut.HasClaim("favouriteBand"), Is.True);
        }
        
        [Test]
        public void IF_user_does_not_have_matching_claim_SHOULD_return_false()
        {
            Assert.That(Sut.HasClaim("BestMeme"), Is.False);
        }
    }
}