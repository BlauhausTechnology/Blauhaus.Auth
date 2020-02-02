using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using Blauhaus.Auth.Abstractions.Builders;
using Blauhaus.Auth.Abstractions.Claims;
using Blauhaus.Auth.Abstractions.Services;
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

        public void Initialize(ClaimsPrincipal claimsPrincipal)
        {

            if (!claimsPrincipal.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var objectIdentifier = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypesExtended.ObjectIdentifierClaimType);
            if (objectIdentifier == null || string.IsNullOrEmpty(objectIdentifier.Value))
            {
                throw new UnauthorizedAccessException("Invalid identity");
            }

            var emails = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "emails");
            if (emails != null && !string.IsNullOrEmpty(emails.Value))
            {
                EmailAddress = emails.Value;
            }

            AuthenticatedUserId = objectIdentifier.Value;
            
            HandleClaimsPrincipal(claimsPrincipal);
        }

        protected virtual void HandleCustomProperties(Dictionary<string, object> deserializedCustomProperties){}
        protected virtual void HandleDefaultProperties(Dictionary<string, object> deserializedCustomProperties){}
        protected virtual void HandleClaimsPrincipal(ClaimsPrincipal claimsPrincipal){}

        public string AuthenticatedUserId { get; private set; }
        public string? EmailAddress { get; protected set; }
    }
}