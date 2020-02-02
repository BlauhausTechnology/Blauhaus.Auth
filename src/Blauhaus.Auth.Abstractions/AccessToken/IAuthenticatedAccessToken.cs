using System.Collections.Generic;

namespace Blauhaus.Auth.Abstractions.AccessToken
{
    public interface IAuthenticatedAccessToken
    {
        void SetAccessToken(string scheme, string authenticatedAccessToken);
        void SetHeader(string key, string value);

        string Scheme { get; }
        string Token { get; }
        IReadOnlyDictionary<string, string> AdditionalHeaders { get; }
        void Clear();

    }
}