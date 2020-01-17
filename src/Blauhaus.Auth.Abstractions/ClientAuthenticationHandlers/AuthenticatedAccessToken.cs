using System.Collections.Generic;
using static System.String;

namespace Blauhaus.Auth.Abstractions.ClientAuthenticationHandlers
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

        public string Scheme { get; protected set; } = Empty;
        public string Token { get; protected set; } = Empty;
        public IReadOnlyDictionary<string, string> AdditionalHeaders => _additionalHeaders;

        public void Clear()
        {
            Scheme = Empty;
            Token = Empty;
            _additionalHeaders.Clear();
        }
    }
}