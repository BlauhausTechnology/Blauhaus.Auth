using System.Collections.Generic;

namespace Blauhaus.Auth.Abstractions.User
{
    public struct UserClaim 
    {
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
    }
}