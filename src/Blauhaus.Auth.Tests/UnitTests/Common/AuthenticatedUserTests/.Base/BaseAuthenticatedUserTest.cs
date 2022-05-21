using System;
using System.Collections.Generic;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Auth.Common.UserFactory;
using Blauhaus.TestHelpers.BaseTests;

namespace Blauhaus.Auth.Tests.UnitTests.Common.AuthenticatedUserTests.Base
{
    public class BaseAuthenticatedUserTest: BaseUnitTest<AuthenticatedUser>
    {
        protected readonly Guid MockUserId = Guid.NewGuid();
        protected readonly string MockEmailAddress = "bob@freever.com";
        protected readonly IReadOnlyList<UserClaim> MockClaims = new List<UserClaim>
        {
            new UserClaim("FavouriteColour", "Blue"),
            new UserClaim("FavouriteBand", "Deep Purple"),
            new UserClaim("myPrefix_Food", "Bananas Apples Pears"),
            new UserClaim("myPrefix_Dogs", "Puppies Squirrels"),
        };

        protected override AuthenticatedUser ConstructSut()
        {
            return new AuthenticatedUser(MockUserId, MockEmailAddress, MockClaims);
        }

    }
}