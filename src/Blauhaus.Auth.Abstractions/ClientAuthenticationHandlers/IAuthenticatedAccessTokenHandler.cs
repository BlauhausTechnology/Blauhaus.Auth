namespace Blauhaus.Auth.Abstractions.ClientAuthenticationHandlers
{
    public interface IAuthenticatedAccessTokenHandler
    {
        void HandleAccessToken(string scheme, string authenticatedAccessToken);
        void ClearAccessToken();
    }
}