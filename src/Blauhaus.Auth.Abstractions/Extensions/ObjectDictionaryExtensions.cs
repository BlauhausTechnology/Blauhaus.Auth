using System.Collections.Generic;

namespace Blauhaus.Auth.Abstractions.Extensions
{
    public static class ObjectDictionaryExtensions
    {
        public static Dictionary<string, object> WithProperty(this Dictionary<string, object> dictionary, string key, object value)
        {
            dictionary[key] = value;
            return dictionary;
        }

        
        public static Dictionary<string, object> WithProperties(this Dictionary<string, object> dictionary, Dictionary<string, string> values)
        {
            foreach (var value in values)
            {
                dictionary[value.Key] = value.Value;
            }
            return dictionary;
        }
    }
}