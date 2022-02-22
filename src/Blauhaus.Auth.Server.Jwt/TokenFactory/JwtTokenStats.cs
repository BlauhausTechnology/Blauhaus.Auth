using System.Security.Claims;

namespace Blauhaus.Auth.Server.Jwt.TokenFactory
{
    public static class JwtTokenStats
    {
        public static IEnumerable < Claim > GetClaims(this JwtToken token, Guid Id) 
        {
            IEnumerable < Claim > claims = new[] {
                new Claim("Id", token.Id.ToString()),
                    new Claim(ClaimTypes.Name, token.Username),
                    new Claim(ClaimTypes.Email, token.EmailId),
                    new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                    new Claim(ClaimTypes.Expiration, DateTime.UtcNow.AddDays(1).ToString("MMM ddd dd yyyy HH:mm:ss tt"))
            };
            return claims;
        }
        public static IEnumerable < Claim > GetClaims(this JwtToken userAccounts, out Guid Id) {
            Id = Guid.NewGuid();
            return GetClaims(userAccounts, Id);
        }
         
    }
}