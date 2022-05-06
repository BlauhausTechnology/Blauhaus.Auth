using System;
using Blauhaus.Analytics.TestHelpers.MockBuilders;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Common.UserFactory;
using Blauhaus.Auth.Server.Ioc;
using Blauhaus.Auth.Server.TokenFactory;
using Blauhaus.Auth.TestHelpers.MockBuilders;
using Blauhaus.Auth.Tests.UnitTests.Base;
using Blauhaus.Time.TestHelpers.MockBuilders;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.Auth.Tests.UnitTests.Server.JwtTokenFactoryTests.Base;

public abstract class BaseJwtTokenFactoryTest : BaseAuthTest<JwtTokenFactory>
{

    protected AuthenticatedUserMockBuilder MockUser = null!;
    protected AuthenticatedUserFactory AuthenticatedUserFactorty = null!;
    protected TimeServiceMockBuilder MockTimeService = null!;

    public override void Setup()
    {
        base.Setup();

        MockUser = new AuthenticatedUserMockBuilder();

        AuthenticatedUserFactorty = new AuthenticatedUserFactory(
            new AnalyticsLoggerMockBuilder<AuthenticatedUserFactory>().Object);
        MockTimeService = new TimeServiceMockBuilder();

        Action<JwtOptions> action = options =>
        {
            options.IssuerSigningKey = Guid.NewGuid().ToString();
            options.ValidAudience = Guid.NewGuid().ToString();
            options.ValidIssuer = Guid.NewGuid().ToString();
            options.ValidFor = TimeSpan.FromHours(2);
        };
        Services.Configure(action);
        AddService(MockTimeService.Object);
    }
     

}