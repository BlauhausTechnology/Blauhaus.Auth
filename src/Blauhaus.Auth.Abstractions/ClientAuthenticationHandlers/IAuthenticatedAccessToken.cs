namespace Blauhaus.Auth.Abstractions.ClientAuthenticationHandlers
{
    public interface IAuthenticatedAccessToken
    {
        void SetAccessToken(string scheme, string authenticatedAccessToken);
        void ClearAccessToken();

        string Scheme { get; }
        string Token { get; }
    }
}