using System.Collections.Generic;

namespace Blauhaus.Auth.Server.Azure.User
{
    public interface IAzureActiveDirectoryUser
    {
        void Initialize(Dictionary<string, object> deserializedAzureObject);
        void PopulateCustomProperties(Dictionary<string, object> deserializedCustomProperties);

        string AuthenticatedUserId { get; }
        string EmailAddress { get; }
    }
}