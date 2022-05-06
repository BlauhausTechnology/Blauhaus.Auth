using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Abstractions.Tokens;
using Blauhaus.Auth.Server.Services;
using Blauhaus.Auth.Server.TokenFactory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Blauhaus.Auth.Server.Ioc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJwtTokenHandlers(this IServiceCollection services, Action<JwtOptions> options)
        {
            services.Configure(options);
            services.AddTransient<IJwtTokenFactory, JwtTokenFactory>();

            var config = new JwtOptions();
            options.Invoke(config);

            services.AddAuthentication(jwtOptions =>
            {
                jwtOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                jwtOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(jwtBearerOptions => {
                    jwtBearerOptions.RequireHttpsMetadata = false;
                    jwtBearerOptions.SaveToken = true;
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters() {
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

        public static IServiceCollection AddPasswordService(this IServiceCollection services, Action<PasswordOptions> options)
        {
            services.AddScoped<IPasswordService, PasswordService>();
            services.Configure(options);
            return services;
        }

    }
}