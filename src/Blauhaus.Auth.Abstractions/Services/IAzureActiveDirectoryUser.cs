using System.Collections.Generic;
using System.Security.Claims;

namespace Blauhaus.Auth.Abstractions.Services
{
    public interface IAzureActiveDirectoryUser
    {
        void Initialize(Dictionary<string, object> deserializedAzureObject);
        void PopulateCustomProperties(Dictionary<string, object> deserializedCustomProperties);
        void Initialize(ClaimsPrincipal claimsPrincipal);

        string AuthenticatedUserId { get; }
        string? EmailAddress { get; }
    }
}