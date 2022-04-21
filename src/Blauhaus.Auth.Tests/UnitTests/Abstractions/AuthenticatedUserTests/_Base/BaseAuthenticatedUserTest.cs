using System;
using System.Collections.Generic;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.TestHelpers.BaseTests;

namespace Blauhaus.Auth.Tests.UnitTests.Abstractions.AuthenticatedUserTests._Base
{
    public class BaseAuthenticatedUserTest: BaseUnitTest<AuthenticatedUser>
    {
        protected readonly Guid MockUserId = Guid.NewGuid();
        protected readonly string MockEmailAddress = "bob@freever.com";
        protected readonly IEnumerable<UserClaim> MockClaims = new List<UserClaim>
        {
            new UserClaim("FavouriteColour", "Blue"),
            new UserClaim("FavouriteBand", "Deep Purple")
        };

        protected override AuthenticatedUser ConstructSut()
        {
            return new AuthenticatedUser(MockUserId, MockEmailAddress, MockClaims);
        }

    }
}