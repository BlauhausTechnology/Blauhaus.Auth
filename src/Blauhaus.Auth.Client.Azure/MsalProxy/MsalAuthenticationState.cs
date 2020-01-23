namespace Blauhaus.Auth.Client.Azure.MsalProxy
{
    public enum MsalAuthenticationState
    {
        Authenticated,
        RequiresLogin,
        RequiresPasswordReset,
        Cancelled,
        Failed
    }
}