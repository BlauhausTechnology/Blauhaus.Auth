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
        protected readonly Dictionary<string, string> MockClaims = new()
        {
            ["FavouriteColour"] =  "Blue",
            ["FavouriteBand"] =   "Deep Purple",
            ["myPrefix_Food"] =   "Bananas Apples Pears",
            ["myPrefix_Dogs"] =   "Puppies Squirrels",
        };

        protected override AuthenticatedUser ConstructSut()
        {
            return new AuthenticatedUser(MockUserId, MockEmailAddress, MockClaims);
        }

    }
}