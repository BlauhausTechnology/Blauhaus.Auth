﻿using Blauhaus.Auth.Abstractions.AccessToken;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Auth.Abstractions.Ioc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterAccessToken(this IServiceCollection services)
        {
            services.AddSingleton<IAuthenticatedAccessToken, AuthenticatedAccessToken>();
            return services;
        }
        
    }
}