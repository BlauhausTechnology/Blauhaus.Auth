using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Blauhaus.Auth.Abstractions.User
{
    public class UserClaim 
    {
        [JsonConstructor]
        public UserClaim(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }

        public override string ToString()
        {
            return $"| {Name}: {Value}";
        }

        public static UserClaim Admin = new UserClaim("Role", "Admin");
        public static UserClaim UserId(Guid userId) => new UserClaim("sub", userId.ToString());
        public static UserClaim EmailAddress(string emailAddress) => new UserClaim("emails", emailAddress);
        public static UserClaim Expiration(DateTime expiration) => new UserClaim(ClaimTypes.Expiration, expiration.ToString("MMM ddd dd yyyy HH:mm:ss tt"));
    }
}