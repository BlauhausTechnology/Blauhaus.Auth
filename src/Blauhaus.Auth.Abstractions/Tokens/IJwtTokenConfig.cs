using System;

namespace Blauhaus.Auth.Abstractions.Tokens
{
    public interface IJwtTokenConfig
    {
        string IssuerSigningKey { get; }
        string ValidIssuer { get; }
        string ValidAudience { get; }
        bool ValidateLifetime { get; }
        bool ValidateAudience { get; }
        bool RequireExpirationTime { get; }
        bool ValidateIssuerSigningKey { get; }
        bool ValidateIssuer { get; }

        TimeSpan ValidFor { get; }
    }
}