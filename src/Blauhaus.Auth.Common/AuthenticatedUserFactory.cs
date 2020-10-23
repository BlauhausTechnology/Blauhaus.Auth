using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Errors;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Responses;

namespace Blauhaus.Auth.Common
{
    public class AuthenticatedUserFactory : IAuthenticatedUserFactory
    {
        private readonly IAnalyticsService _analyticsService;

        public AuthenticatedUserFactory(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }
        
        public Response<IAuthenticatedUser> Create(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(token))
            {
                return _analyticsService.TraceErrorResponse<IAuthenticatedUser>(this, AuthErrors.InvalidToken);
            }
            var accessToken = tokenHandler.ReadJwtToken(token);
            return ExtractClaims(accessToken.Claims);
        }

        public Response<IAuthenticatedUser> Create(ClaimsPrincipal claimsPrincipal)
        { 
            if (!claimsPrincipal.Identity.IsAuthenticated)
            {
                return _analyticsService.TraceErrorResponse<IAuthenticatedUser>(this, AuthErrors.NotAuthenticated);
            }
             
            return ExtractClaims(claimsPrincipal.Claims);
        }

        private Response<IAuthenticatedUser> ExtractClaims(IEnumerable<Claim> claims)
        {
            string emailAddress = null;
            var userId = Guid.Empty;
            var userClaims = new List<UserClaim>();
            var authPolicy = string.Empty;
            var scopes = new string[0];

            foreach (var claim in claims)
            {
                if (claim.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier" || claim.Type == "sub")
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
                return _analyticsService.TraceErrorResponse<IAuthenticatedUser>(this, AuthErrors.InvalidIdentity);
            }
            
            var user = (IAuthenticatedUser) new AuthenticatedUser(userId, emailAddress, userClaims, authPolicy, scopes);
            
            return Response.Success(user);
        }
    }
}