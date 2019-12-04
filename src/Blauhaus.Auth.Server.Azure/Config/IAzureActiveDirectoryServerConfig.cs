namespace Blauhaus.Auth.Server.Azure.Config
{
    public interface IAzureActiveDirectoryServerConfig
    {
        //ref https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-devquickstarts-graph-dotnet?tabs=applications

        string ApplicationId { get; }
        string ExtensionsApplicationId { get; }
        string TenantId { get; }
        string ClientSecret { get; }
        
        string AuthorityBaseUrl { get; }
        string GraphResourceId { get; }
        string GraphEndpoint { get; }
        string GraphVersion { get; }
    }
}