using System.Collections.Generic;

namespace Blauhaus.Auth.Server.Azure.User
{
    public interface IAzureActiveDirectoryUser
    {
        void Initialize(Dictionary<string, object> deserializedAzureObject);
        void PopulateCustomProperties(Dictionary<string, object> deserializedCustomProperties);

        string UserObjectId { get; }
        string EmailAddress { get; }
    }
}