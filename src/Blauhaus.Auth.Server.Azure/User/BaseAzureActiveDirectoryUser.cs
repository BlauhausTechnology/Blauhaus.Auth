using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Blauhaus.Auth.Server.Azure.User
{
    public abstract class BaseAzureActiveDirectoryUser : IAzureActiveDirectoryUser
    {
        public void Initialize(Dictionary<string, object> deserializedAzureObject)
        {
            AuthenticatedUserId = (string) deserializedAzureObject["objectId"];

            if (deserializedAzureObject.TryGetValue("signInNames", out var signInNames))
            {

                if (signInNames is JArray signInNameProperties)
                {
                    foreach (var signInNameProperty in signInNameProperties)
                    {
                        var key = (string)signInNameProperty.First.Value<JProperty>().Value;
                        if (key == "emailAddress")
                        {
                            EmailAddress = signInNameProperty.Last.Value<JProperty>().Value.ToString();
                        }
                    }
                }
            }

            HandleDefaultProperties(deserializedAzureObject);
        }

        public void PopulateCustomProperties(Dictionary<string, object> deserializedCustomProperties)
        {
            HandleCustomProperties(deserializedCustomProperties);
        }

        protected virtual void HandleCustomProperties(Dictionary<string, object> deserializedCustomProperties){}
        protected virtual void HandleDefaultProperties(Dictionary<string, object> deserializedCustomProperties){}


        public string AuthenticatedUserId { get; private set; }
        public string? EmailAddress { get; protected set; }
    }
}