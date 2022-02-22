using Blauhaus.Auth.Abstractions.Tokens;

namespace Blauhaus.Auth.Server.Jwt.Ioc
{
    public abstract class BaseJwtTokenConfig : IJwtTokenConfig
    {
        protected BaseJwtTokenConfig(
            string issuerSigningKey, string validIssuer, string validAudience, TimeSpan validFor, 
            bool requireExpirationTime,  bool validateIssuerSigningKey,  bool validateAudience = true, bool validateIssuer = true, bool validateLifetime = true)
        {
            IssuerSigningKey = issuerSigningKey;
            ValidIssuer = validIssuer;
            ValidAudience = validAudience;
            ValidateAudience = validateAudience;
            RequireExpirationTime = requireExpirationTime;
            ValidateLifetime = validateLifetime;
            ValidateIssuerSigningKey = validateIssuerSigningKey;
            ValidFor = validFor;
            ValidateIssuer = validateIssuer;
        }

        public string IssuerSigningKey { get; }
        public string ValidIssuer { get; }
        public string ValidAudience { get; }
        public TimeSpan ValidFor { get; }
        
        public bool ValidateLifetime { get; } 
        public bool ValidateAudience { get;}  
        public bool RequireExpirationTime { get; }
        public bool ValidateIssuerSigningKey { get; }
        public bool ValidateIssuer { get; }
    }
}