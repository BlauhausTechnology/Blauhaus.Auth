using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Blauhaus.Auth.Abstractions.User
{
    public interface IAuthenticatedUser
    {
        Guid UserId { get; }   
        string? EmailAddress { get; }
        IReadOnlyList<Claim> Claims { get; }


    }
}