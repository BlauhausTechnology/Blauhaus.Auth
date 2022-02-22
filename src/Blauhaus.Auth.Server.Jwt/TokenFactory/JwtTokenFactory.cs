﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blauhaus.Auth.Abstractions.Tokens;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Auth.Common.Extensions;
using Blauhaus.Time.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace Blauhaus.Auth.Server.Jwt.TokenFactory
{
    public class JwtTokenFactory
    {
        private readonly IJwtTokenConfig _config;
        private readonly ITimeService _timeService;

        private readonly SigningCredentials _singingCredentials;

        public JwtTokenFactory(
            IJwtTokenConfig config,
            ITimeService timeService)
        {
            _config = config;
            _timeService = timeService;

            var signingKey = System.Text.Encoding.ASCII.GetBytes(_config.IssuerSigningKey);
            _singingCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256);
        }

        public string GenerateTokenForUser(IAuthenticatedUser authenticatedUser)
        {
            var now = _timeService.CurrentUtcTime;
            var expiresAt = now.Add(_config.ValidFor);

            var claims = new List<Claim>
            {
                UserClaim.UserId(authenticatedUser.UserId).ToClaim()
            };

            if(authenticatedUser.EmailAddress!=null)
                claims.Add(UserClaim.EmailAddress(authenticatedUser.EmailAddress).ToClaim());

            foreach (var authenticatedUserClaim in authenticatedUser.Claims)
            {
                claims.Add(authenticatedUserClaim.ToClaim());
            }

            var jwtToken = new JwtSecurityToken(
                issuer: _config.ValidIssuer,
                audience: _config.ValidAudience,
                notBefore: now,
                claims: claims,
                expires: expiresAt,
                signingCredentials: _singingCredentials);

            var stringToken =  new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return stringToken;
        } 
    }
}