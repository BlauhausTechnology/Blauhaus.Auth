namespace Blauhaus.Auth.Client.Service.Handlers
{
    public interface IAuthenticatedUserIdHandler
    {
        void HandleUserId(string authenticatedUserId);
        void ClearUserId();
    }
}