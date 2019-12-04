using System.Collections.Generic;

namespace Blauhaus.Auth.Server.Azure.User
{
    public abstract class BaseAzureActiveDirectoryUser : IAzureActiveDirectoryUser
    {
        private Dictionary<string, object> _customProperties;

        public void Initialize(Dictionary<string, object> deserializedAzureObject)
        {
            UserObjectId = (string) deserializedAzureObject["objectId"];
        }

        public void PopulateCustomProperties(Dictionary<string, object> deserializedCustomProperties)
        {
            _customProperties = deserializedCustomProperties;
        }

        public string UserObjectId { get; private set; }
        public string EmailAddress { get; private set; }
    }
}