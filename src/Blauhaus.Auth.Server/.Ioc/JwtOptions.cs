namespace Blauhaus.Auth.Server.Ioc;

public class JwtOptions
{
    public string IssuerSigningKey { get; set; }
    public string ValidIssuer { get; set; }
    public string ValidAudience { get; set; }
    public TimeSpan ValidFor { get; set; }
        
    public bool ValidateLifetime { get; set; } 
    public bool ValidateAudience { get; set; }   
    public bool RequireExpirationTime { get; set; }
    public bool ValidateIssuerSigningKey { get; set; }
    public bool ValidateIssuer { get; set; }
}