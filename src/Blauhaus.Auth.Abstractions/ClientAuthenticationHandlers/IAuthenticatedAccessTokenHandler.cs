namespace Blauhaus.Auth.Client.Service.Handlers
{
    public interface IAuthenticatedAccessTokenHandler
    {
        void HandleAccessToken(string authenticatedAccessToken);
        void ClearAccessToken();
    }
}