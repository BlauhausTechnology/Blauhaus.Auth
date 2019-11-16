namespace Blauhaus.Auth.Abstractions.ClientAuthenticationHandlers
{
    public interface IAuthenticatedAccessTokenHandler
    {
        void HandleAccessToken(string authenticatedAccessToken);
        void ClearAccessToken();
    }
}