using System;
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

        public static UserClaim Admin = new("Role", "Admin");
        public static UserClaim UserId(Guid userId) => new("sub", userId.ToString());
        public static UserClaim EmailAddress(string emailAddress) => new("emails", emailAddress);
        public static UserClaim Expiration(DateTime expiration) => new(ClaimTypes.Expiration, expiration.ToString("MMM ddd dd yyyy HH:mm:ss tt"));
    }
}