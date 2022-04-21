using System;
using System.Collections.Generic;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.TestHelpers.MockBuilders;

namespace Blauhaus.Auth.TestHelpers.MockBuilders
{
    public class AuthenticatedUserMockBuilder : BaseMockBuilder<AuthenticatedUserMockBuilder, IAuthenticatedUser>
    {
        public AuthenticatedUserMockBuilder(Guid? userId = null)
        {
            var id = userId ?? Guid.NewGuid();
            With(x => x.EmailAddress, Guid.NewGuid() + "@freever.com");
            With(x => x.UserId, id);
            With(x => x.Claims, new List<UserClaim>());
        }

        public AuthenticatedUserMockBuilder With_UserId(Guid id)
        {
            With(x => x.UserId, id);
            return this;
        }
        public AuthenticatedUserMockBuilder With_Claim(UserClaim claim)
        {
            Mock.Setup(x => x.HasClaim(claim.Name)).Returns(true);
            Mock.Setup(x => x.HasClaimValue(claim.Name, claim.Value)).Returns(true);
            With(x => x.Claims, new List<UserClaim>{claim});
            return this;
        }
    }
}