using System.Collections.Generic;

namespace Blauhaus.Auth.Server.Azure.User
{
    public abstract class BaseAzureActiveDirectoryUser : IAzureActiveDirectoryUser
    {
        public void Initialize(Dictionary<string, object> deserializedAzureObject)
        {
            //todo get email address?
            AuthenticatedUserId = (string) deserializedAzureObject["objectId"];
            HandleDefaultProperties(deserializedAzureObject);
        }

        public void PopulateCustomProperties(Dictionary<string, object> deserializedCustomProperties)
        {
            HandleCustomProperties(deserializedCustomProperties);
        }

        protected virtual void HandleCustomProperties(Dictionary<string, object> deserializedCustomProperties){}
        protected virtual void HandleDefaultProperties(Dictionary<string, object> deserializedCustomProperties){}


        public string AuthenticatedUserId { get; private set; }
        public string EmailAddress { get; protected set; }
    }
}