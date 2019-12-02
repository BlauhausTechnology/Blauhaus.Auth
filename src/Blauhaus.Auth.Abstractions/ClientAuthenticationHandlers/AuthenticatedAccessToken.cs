using static System.String;

namespace Blauhaus.Auth.Abstractions.ClientAuthenticationHandlers
{
    public class AuthenticatedAccessToken : IAuthenticatedAccessToken
    {


        public void SetAccessToken(string scheme, string authenticatedAccessToken)
        {
            Scheme = scheme;
            Token = authenticatedAccessToken;
        }

        public void ClearAccessToken()
        {
            Scheme = Empty;
            Token = Empty;
        }

        public string Scheme { get; protected set; } = Empty;
        public string Token { get; protected set; } = Empty;
    }
}