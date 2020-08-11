using System;
using System.Collections.Generic;
using System.Security.Claims;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Errors;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Abstractions.User;
using CSharpFunctionalExtensions;

namespace Blauhaus.Auth.Server.Azure.Service
{
    public class AzureAuthenticatedUserFactory : IAzureAuthenticatedUserFactory
    {
        private readonly IAnalyticsService _analyticsService;

        public AzureAuthenticatedUserFactory(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public Result<IAuthenticatedUser> Create(ClaimsPrincipal claimsPrincipal)
        {
            string emailAddress = null;
            var userId = Guid.Empty;
            var userClaims = new List<UserClaim>();

            if (!claimsPrincipal.Identity.IsAuthenticated)
            {
                return _analyticsService.TraceErrorResult<IAuthenticatedUser>(this, AuthErrors.NotAuthenticated);
            }

            foreach (var claim in claimsPrincipal.Claims)
            {
                if (claim.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")
                {
                    Guid.TryParse(claim.Value, out userId);
                }
                else if (claim.Type == "emails")
                {
                    emailAddress = string.IsNullOrWhiteSpace(claim.Value) ? null : claim.Value;
                }
                else if (claim.Type.StartsWith("extension_"))
                {
                    var claimName = claim.Type.Replace("extension_", "");
                    userClaims.Add(new UserClaim(claimName, claim.Value));
                }
            }

            if (userId == Guid.Empty)
            {
                return _analyticsService.TraceErrorResult<IAuthenticatedUser>(this, AuthErrors.InvalidIdentity);
            }

            var user = new AuthenticatedUser(userId, emailAddress, userClaims);

            _analyticsService.Trace(this, "User profile extracted from ClaimsPrincipal: " + user.UserId);
            
            return user;
        }
    }
}