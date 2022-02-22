using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Abstractions.Tokens;
using Blauhaus.Auth.Common.UserFactory;
using Blauhaus.Auth.Server.Jwt.TokenFactory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace Blauhaus.Auth.Server.Jwt.Ioc
{
    public static class ServiceCollectionExtensions
    {
         
        public static IServiceCollection AddJwtTokenHandlers(this IServiceCollection services, IJwtTokenConfig config) 
        {
            services.AddSingleton(config);
            services.AddTransient<IJwtTokenFactory, JwtTokenFactory>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options => {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters() {
                    ValidateIssuerSigningKey = config.ValidateIssuerSigningKey,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config.IssuerSigningKey)),
                    ValidateIssuer = config.ValidateIssuer,
                    ValidIssuer = config.ValidIssuer,
                    ValidateAudience = config.ValidateAudience,
                    ValidAudience = config.ValidAudience,
                    RequireExpirationTime = config.RequireExpirationTime,
                    ValidateLifetime = config.RequireExpirationTime, 
                };
            });;

            return services;
        }

    }
}