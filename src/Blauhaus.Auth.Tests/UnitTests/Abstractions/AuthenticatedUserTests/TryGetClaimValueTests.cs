using Blauhaus.Auth.Tests.UnitTests.Abstractions.AuthenticatedUserTests._Base;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Abstractions.AuthenticatedUserTests
{
    public class TryGetClaimValueTests : BaseAuthenticatedUserTest
    {

        [Test]
        public void IF_user_has_matching_claim_SHOULD_return_true()
        {
            Assert.That(Sut.TryGetClaimValue("FavouriteColour", out var colour), Is.True);
            Assert.That(colour, Is.EqualTo("Blue"));
            Assert.That(Sut.TryGetClaimValue("FavouriteBand", out var band), Is.True);
            Assert.That(band, Is.EqualTo("Deep Purple"));
        }

        [Test]
        public void IF_user_has_matching_claim_in_different_case_SHOULD_return_true()
        {
            Assert.That(Sut.TryGetClaimValue("Favouritecolour", out var colour), Is.True);
            Assert.That(colour, Is.EqualTo("Blue"));
            Assert.That(Sut.TryGetClaimValue("Favouriteband", out var band), Is.True);
            Assert.That(band, Is.EqualTo("Deep Purple"));
        }

    
        [Test]
        public void IF_user_does_not_have_matching_claim_SHOULD_return_false()
        {
            Assert.That(Sut.TryGetClaimValue("BestMeme", out var meme), Is.False);
            Assert.That(meme, Is.EqualTo(string.Empty));
        }
    }
}