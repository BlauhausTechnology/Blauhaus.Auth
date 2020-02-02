using System.Threading;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.AccessToken;
using Blauhaus.Auth.Client.Azure.MsalProxy;
using Blauhaus.Auth.Client.Azure.Service;
using Blauhaus.Auth.Tests.Builders;
using Blauhaus.Common.TestHelpers;
using Blauhaus.Ioc.Abstractions;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Client.AzureAuthenticationClientServiceTests._Base
{
    public class BaseAuthenticationClientServiceTest : BaseUnitTest<AzureAuthenticationClientService>
    {

        protected MockBuilder<IAnalyticsService> MockAnalyticsService;
        protected MockBuilder<IIocService> MockIocService;
        internal MsalClientProxyMockBuilder MockMsalClientProxy;
        internal MockBuilder<IAuthenticatedAccessToken> MockAuthenticatedAccessToken;
        protected CancellationToken MockCancelToken;

        protected readonly MsalClientResult MockAuthenticatedUserResult = MsalClientResult.Authenticated(new AuthenticationResultBuilder()
            .With_AccessToken("authenticatedAccesstoken")
            .With_UniqueId("authenticatedUserId").Build());

        [SetUp]
        public virtual void Setup()
        {
            Cleanup();
            MockAnalyticsService = new MockBuilder<IAnalyticsService>();
            MockMsalClientProxy = new MsalClientProxyMockBuilder();
            MockAuthenticatedAccessToken = new MockBuilder<IAuthenticatedAccessToken>();
            MockIocService = new MockBuilder<IIocService>();

            MockCancelToken = new CancellationTokenSource().Token;
            
            MockMsalClientProxy.Where_AuthenticateSilentlyAsync_returns(MockAuthenticatedUserResult);

            MockIocService.Mock.Setup(x => x.Resolve<IMsalClientProxy>())
                .Returns(MockMsalClientProxy.Object);
        }

        protected override AzureAuthenticationClientService ConstructSut()
        {
            return new AzureAuthenticationClientService(
                MockAnalyticsService.Object,
                MockIocService.Object,
                MockAuthenticatedAccessToken.Object);
        }
    }
}