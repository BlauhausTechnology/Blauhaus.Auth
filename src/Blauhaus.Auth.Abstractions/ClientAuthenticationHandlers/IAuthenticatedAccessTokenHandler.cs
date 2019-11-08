namespace Blauhaus.Auth.Client.Service.Handlers
{
    public interface IAuthenticatedAccessTokenHandler
    {
        void Handle(string authenticatedAccessToken);
    }
}