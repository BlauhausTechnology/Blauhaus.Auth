using System.Collections.Generic;

namespace Blauhaus.Auth.Abstractions.Extensions
{
    public static class ObjectExtensions
    {
        public static Dictionary<string, object> ToObjectDictionary(this object value, string key)
        {
            return new Dictionary<string, object>
            {
                {key, value }
            };
        }
    }
}