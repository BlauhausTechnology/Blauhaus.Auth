namespace Blauhaus.Auth.Abstractions.ClientAuthenticationHandlers
{
    public class DefaultAccessTokenHandler : IAuthenticatedAccessTokenHandler
    {
        public void HandleAccessToken(string authenticatedAccessToken)
        {
        }

        public void HandleAccessToken(string scheme, string authenticatedAccessToken)
        {
            
        }

        public void ClearAccessToken()
        {
        }
    }
}