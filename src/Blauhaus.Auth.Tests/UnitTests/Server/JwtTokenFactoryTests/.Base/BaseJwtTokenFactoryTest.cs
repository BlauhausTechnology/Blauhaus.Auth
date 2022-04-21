using Blauhaus.Analytics.TestHelpers.MockBuilders;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Common.UserFactory;
using Blauhaus.Auth.Server.Jwt.TokenFactory;
using Blauhaus.Auth.TestHelpers.MockBuilders;
using Blauhaus.Auth.Tests.UnitTests.Base;
using Blauhaus.Time.TestHelpers.MockBuilders;

namespace Blauhaus.Auth.Tests.UnitTests.Server.JwtTokenFactoryTests.Base;

public abstract class BaseJwtTokenFactoryTest : BaseAuthTest<JwtTokenFactory>
{

    protected AuthenticatedUserMockBuilder MockUser = null!;
    protected AuthenticatedUserFactory AuthenticatedUserFactorty = null!;
    protected JwtTokenConfigMockBuilder MockJwtTokenConfig = null!;
    protected TimeServiceMockBuilder MockTimeService = null!;

    public override void Setup()
    {
        base.Setup();

        MockUser = new AuthenticatedUserMockBuilder();
        MockJwtTokenConfig = new JwtTokenConfigMockBuilder();
        AuthenticatedUserFactorty = new AuthenticatedUserFactory(
            new AnalyticsLoggerMockBuilder<AuthenticatedUserFactory>().Object);
        MockTimeService = new TimeServiceMockBuilder();

        AddService(MockJwtTokenConfig.Object);
        AddService(MockTimeService.Object);
    }
     

}