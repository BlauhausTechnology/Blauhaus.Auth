using System;
using Blauhaus.Auth.Abstractions.Tokens;
using Blauhaus.TestHelpers.MockBuilders;

namespace Blauhaus.Auth.TestHelpers.MockBuilders;

public class JwtTokenConfigMockBuilder : BaseMockBuilder<JwtTokenConfigMockBuilder, IJwtTokenConfig>
{
    public JwtTokenConfigMockBuilder()
    {
        With(x => x.IssuerSigningKey, Guid.NewGuid().ToString());
        With(x => x.ValidIssuer, Guid.NewGuid().ToString());
        With(x => x.ValidAudience, Guid.NewGuid().ToString());
        With(x => x.ValidFor, TimeSpan.FromMinutes(1));
    }
}