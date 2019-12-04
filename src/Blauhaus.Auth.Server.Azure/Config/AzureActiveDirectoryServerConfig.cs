namespace Blauhaus.Auth.Server.Azure.Config
{
    public class AzureActiveDirectoryServerConfig : IAzureActiveDirectoryServerConfig
    {
        public AzureActiveDirectoryServerConfig(string tenantId, string applicationId,  string extensionsApplicationId, string clientSecret)
        {
            ApplicationId = applicationId;
            TenantId = tenantId;
            ClientSecret = clientSecret;
            ExtensionsApplicationId = extensionsApplicationId;

            AuthorityBaseUrl = "https://login.microsoftonline.com/";
            GraphResourceId = "https://graph.windows.net/";
            GraphEndpoint = "https://graph.windows.net/";
            GraphVersion = "api-version=1.6";
        }
        
        public string ApplicationId { get; }
        public string ExtensionsApplicationId { get; }
        public string TenantId { get; }
        public string ClientSecret { get; }

        public string AuthorityBaseUrl { get; protected set; }
        public string GraphResourceId { get; protected set; }
        public string GraphEndpoint { get; protected set; }
        public string GraphVersion { get; protected set; }
    }
}