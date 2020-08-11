using System;
using Blauhaus.Auth.Abstractions.Models;
using Blauhaus.TestHelpers.MockBuilders;

namespace Blauhaus.Auth.TestHelpers.MockBuilders
{
    public class UserAuthenticationMockBuilder : BaseMockBuilder<UserAuthenticationMockBuilder, IUserAuthentication>
    {
        public UserAuthenticationMockBuilder(bool isAuthenticated = true)
        {
            if (isAuthenticated)
            {
                Authenticated();
            }
            else
            {
                NotAuthenticated();
            }
        }

        public UserAuthenticationMockBuilder Authenticated()
        {
            With(x => x.AuthenticatedAccessToken, "Access_" + Guid.NewGuid());
            With(x => x.AuthenticatedIdToken, "Id_" + Guid.NewGuid());
            With(x => x.AuthenticationMode, AuthenticationMode.SilentLogin);
            With(x => x.AuthenticationState, UserAuthenticationState.Authenticated);
            With(x => x.IsAuthenticated, true);
            With(x => x.ErrorMessage, null);
            With(x => x.User, new AuthenticatedUserMockBuilder().Object);
            return this;
        }
        public UserAuthenticationMockBuilder NotAuthenticated()
        {
            With(x => x.AuthenticatedAccessToken, "");
            With(x => x.AuthenticatedIdToken, "");
            With(x => x.AuthenticationMode, AuthenticationMode.SilentLogin);
            With(x => x.AuthenticationState, UserAuthenticationState.Failed);
            With(x => x.IsAuthenticated, false);
            With(x => x.ErrorMessage, "Bad thing");
            With(x => x.User, null);
            return this;
        }
    }
}