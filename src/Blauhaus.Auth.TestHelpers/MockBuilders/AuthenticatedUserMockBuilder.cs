using System;
using System.Collections.Generic;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.TestHelpers.MockBuilders;

namespace Blauhaus.Auth.TestHelpers.MockBuilders
{
    public class AuthenticatedUserMockBuilder : BaseMockBuilder<AuthenticatedUserMockBuilder, IAuthenticatedUser>
    {

        private readonly List<UserClaim> _claims = new();

        public AuthenticatedUserMockBuilder(Guid? userId = null)
        {
            var id = userId ?? Guid.NewGuid();
            With(x => x.EmailAddress, Guid.NewGuid() + "@freever.com");
            With(x => x.UserId, id);
            With(x => x.Claims, _claims);
        }
        public AuthenticatedUserMockBuilder With_EmailAddress(string emailAddress)
        {
            With(x => x.EmailAddress, emailAddress);
            return this;
        }
        public AuthenticatedUserMockBuilder With_UserId(Guid id)
        {
            With(x => x.UserId, id);
            return this;
        }
        public AuthenticatedUserMockBuilder With_Claim(UserClaim claim)
        {
            var val = claim.Value;

            Mock.Setup(x => x.HasClaim(claim.Name)).Returns(true);
            Mock.Setup(x => x.HasClaimValue(claim.Name, claim.Value)).Returns(true);
            Mock.Setup(x => x.TryGetClaimValue(claim.Name, out val)).Returns(true);
            
            _claims.Add(claim);
            With(x => x.Claims, _claims);
            
            return this;
        }
    }
}