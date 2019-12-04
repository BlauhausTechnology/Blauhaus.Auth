namespace Blauhaus.Auth.Client.Azure.Config
{
    public interface IAzureActiveDirectoryClientConfig
    {
        string AuthBaseUrl { get; }
        string TenantName { get; }
        string ApplicationId { get; }
        string TenantId { get; }
        string IosKeychainSecurityGroups { get; }
        string[] Scopes { get; }
        string SigninPolicyName { get; }
        string ResetPasswordPolicyName { get; }

        string AuthoritySignin { get; }
        string AuthorityPasswordReset { get; }
    }
}