namespace Blauhaus.Auth.Client.Azure.Config
{
    public class AzureActiveDirectoryClientConfig : IAzureActiveDirectoryClientConfig
    {
        public AzureActiveDirectoryClientConfig(
            string tenantName,
            string tenantId,
            string applicationId,
            string iosKeychainSecurityGroups,
            string[] scopes,
            string signinPolicyName,
            string resetPasswordPolicyName)
        {
            SigninPolicyName = signinPolicyName;
            ResetPasswordPolicyName = resetPasswordPolicyName;
            TenantName = tenantName;
            ApplicationId = applicationId;
            TenantId = tenantId;
            IosKeychainSecurityGroups = iosKeychainSecurityGroups;
            Scopes = scopes;

            AuthBaseUrl = $"https://{TenantName}.b2clogin.com/tfp/{TenantId}/";
            AuthoritySignin = $"{AuthBaseUrl}{SigninPolicyName}";
            AuthorityPasswordReset = $"{AuthBaseUrl}{ResetPasswordPolicyName}";
            
        }

        public string AuthBaseUrl { get; }
        public string TenantName { get; }
        public string ApplicationId { get; }
        public string TenantId { get; }
        public string IosKeychainSecurityGroups { get; }
        public string[] Scopes { get; }
        public string SigninPolicyName { get; }
        public string ResetPasswordPolicyName{ get; }
        public string AuthoritySignin { get; }
        public string AuthorityPasswordReset { get; }

        
    }
}