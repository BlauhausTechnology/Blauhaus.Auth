using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Errors;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Responses;
using CSharpFunctionalExtensions;

namespace Blauhaus.Auth.Server.Azure.Service
{
    public class AuthenticatedUserFactory : IAuthenticatedUserFactory
    {
        private readonly IAnalyticsService _analyticsService;

        public AuthenticatedUserFactory(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public Response<IAuthenticatedUser> Create(ClaimsPrincipal claimsPrincipal)
        {
            string emailAddress = null;
            var userId = Guid.Empty;
            var userClaims = new List<UserClaim>();
            var authPolicy = string.Empty;
            var scopes = new string[0];

            if (!claimsPrincipal.Identity.IsAuthenticated)
            {
                return _analyticsService.TraceErrorResponse<IAuthenticatedUser>(this, AuthErrors.NotAuthenticated);
            }

            foreach (var claim in claimsPrincipal.Claims)
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

            _analyticsService.Trace(this, "User profile extracted from ClaimsPrincipal: " + user.UserId);
            
            return Response.Success(user);
        }
    }
}