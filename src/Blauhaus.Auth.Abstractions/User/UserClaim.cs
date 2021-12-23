using System.Collections.Generic;
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
    }
}