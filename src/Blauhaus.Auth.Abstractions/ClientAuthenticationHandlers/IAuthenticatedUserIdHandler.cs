namespace Blauhaus.Auth.Client.Service.Handlers
{
    public interface IAuthenticatedUserIdHandler
    {
        void Handle(string authenticatedUserId);
    }
}