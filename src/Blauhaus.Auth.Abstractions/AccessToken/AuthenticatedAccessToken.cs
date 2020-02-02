using System;
using System.Collections.Generic;

namespace Blauhaus.Auth.Abstractions.AccessToken
{
    public class AuthenticatedAccessToken : IAuthenticatedAccessToken
    {
        private readonly Dictionary<string, string> _additionalHeaders = new Dictionary<string, string>();


        public void SetAccessToken(string scheme, string authenticatedAccessToken)
        {
            Scheme = scheme;
            Token = authenticatedAccessToken;
        }

        public void SetHeader(string key, string value)
        {
            _additionalHeaders[key] = value;
        }

        public string Scheme { get; protected set; } = String.Empty;
        public string Token { get; protected set; } = String.Empty;
        public IReadOnlyDictionary<string, string> AdditionalHeaders => _additionalHeaders;

        public void Clear()
        {
            Scheme = String.Empty;
            Token = String.Empty;
            _additionalHeaders.Clear();
        }
    }
}