using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Blauhaus.Auth.Abstractions.Services
{
    public interface IAuthenticatedUser
    {
        Guid UserId { get; }     //todo make Guid
        string? EmailAddress { get; }
        IList<Claim> Claims { get; }



        //todo remove from interface
        //void Initialize(Dictionary<string, object> deserializedAzureObject);
        //void PopulateCustomProperties(Dictionary<string, object> deserializedCustomProperties);
        //void Initialize(ClaimsPrincipal claimsPrincipal);

    }
}