using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Auth.Abstractions.Errors;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Responses;

namespace Blauhaus.Auth.Common.UserFactory
{
    public class AuthenticatedUserFactory : IAuthenticatedUserFactory
    {
        private readonly IAnalyticsLogger<AuthenticatedUserFactory> _logger;

        public AuthenticatedUserFactory(
            IAnalyticsLogger<AuthenticatedUserFactory> logger)
        {
            _logger = logger;
        }
        
        public Response<IAuthenticatedUser> ExtractFromJwtToken(string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(jwtToken))
            {
                return _logger.LogErrorResponse<IAuthenticatedUser>(AuthError.InvalidToken);
            }
            var accessToken = tokenHandler.ReadJwtToken(jwtToken);
            return ExtractClaims(accessToken.Claims);
        }

        public Response<IAuthenticatedUser> ExtractFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
        { 
            if (!claimsPrincipal.Identity.IsAuthenticated)
            {
                return _logger.LogErrorResponse<IAuthenticatedUser>(AuthError.NotAuthenticated);
            }
             
            return ExtractClaims(claimsPrincipal.Claims);
        }

        private Response<IAuthenticatedUser> ExtractClaims(IEnumerable<Claim> claims)
        {
            string emailAddress = null;
            var userId = Guid.Empty;
            var userClaims = new List<UserClaim>();
            var authPolicy = string.Empty;
            var scopes = Array.Empty<string>();

            foreach (var claim in claims)
            {
                if (claim.Type is "http://schemas.microsoft.com/identity/claims/objectidentifier" or "sub")
                {
                    Guid.TryParse(claim.Value, out userId);
                }
                else if (claim.Type == "emails")
                {
                    emailAddress = string.IsNullOrWhiteSpace(claim.Value) ? null : claim.Value;
                }
                else if (claim.Type == "tfp")
                {
                    authPolicy = claim.Value;
                }
                else if (claim.Type == "scp")
                {
                    scopes = claim.Value.Split(' ').ToArray();
                }
                else if (claim.Type.StartsWith("extension_"))
                {
                    var claimName = claim.Type.Replace("extension_", "");
                    userClaims.Add(new UserClaim(claimName, claim.Value));
                }
            }

            if (userId == Guid.Empty)
            {
                return _logger.LogErrorResponse<IAuthenticatedUser>(AuthError.InvalidIdentity);
            }
            
            var user = (IAuthenticatedUser) new AuthenticatedUser(userId, emailAddress, userClaims, authPolicy, scopes);
            
            return Response.Success(user);
        }
    }
}