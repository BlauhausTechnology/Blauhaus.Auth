using System;
using System.Collections.Generic;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.Auth.TestHelpers.MockBuilders
{
    public class AuthenticatedUserMockBuilder : BaseMockBuilder<AuthenticatedUserMockBuilder, IAuthenticatedUser>
    {

        private readonly Dictionary<string, string> _properties = new();

        public AuthenticatedUserMockBuilder(Guid? userId = null)
        {
            var id = userId ?? Guid.NewGuid();
            With(x => x.EmailAddress, Guid.NewGuid() + "@freever.com");
            With(x => x.UserId, id);
            With(x => x.Properties, _properties);
        }
        public AuthenticatedUserMockBuilder With_EmailAddress(string emailAddress)
        {
            With(x => x.EmailAddress, emailAddress);
            return this;
        }
        public AuthenticatedUserMockBuilder Where_GetClaimValuesByPrefix_Returns(Dictionary<string, string> values)
        {
            base.Mock.Setup(x => x.GetClaimValuesByPrefix(It.IsAny<string>()))
                .Returns(values);
            return this;
        }
        public AuthenticatedUserMockBuilder Where_GetClaimValuesByPrefix_Returns(Dictionary<string, string> values, string prefix)
        {
            base.Mock.Setup(x => x.GetClaimValuesByPrefix(prefix))
                .Returns(values);
            return this;
        }
        public AuthenticatedUserMockBuilder With_UserId(Guid id)
        {
            With(x => x.UserId, id);
            return this;
        }
        public AuthenticatedUserMockBuilder With_Claim(string name, string value)
        {

            Mock.Setup(x => x.HasClaim(name)).Returns(true);
            Mock.Setup(x => x.HasClaimValue(name, value)).Returns(true);
            Mock.Setup(x => x.TryGetClaimValue(name, out value)).Returns(true);
            
            _properties[name] = value;
            With(x => x.Properties, _properties);
            
            return this;
        }
    }
}