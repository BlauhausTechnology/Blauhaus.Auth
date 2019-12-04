using System.Threading;
using Blauhaus.Auth.Abstractions.ClientAuthenticationHandlers;
using Blauhaus.Auth.Client.Azure.Config;
using Blauhaus.Auth.Client.Azure.MsalProxy;
using Blauhaus.Auth.Client.Azure.Service;
using Blauhaus.Auth.Tests.Builders;
using Blauhaus.Common.TestHelpers;
using Blauhaus.Common.Time.Service;
using Blauhaus.Ioc.Abstractions;
using Blauhaus.Loggers.Common.Abstractions;
using Microsoft.Identity.Client;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Client.AzureAuthenticationClientServiceTests._Base
{
    public class BaseAuthenticationClientServiceTest : BaseUnitTest<AzureAuthenticationClientService>
    {

        protected MockBuilder<ILogService> MockLogService;
        protected MockBuilder<IIocService> MockIocService;
        protected MockBuilder<ITimeService> MockTimeService;
        internal MockBuilder<IMsalClientProxy> MockMsalClientProxy;
        internal MockBuilder<IAuthenticatedAccessToken> MockAuthenticatedAccessToken;
        protected CancellationToken CancelToken;

        protected readonly MsalClientResult AuthenticatedUserResult = MsalClientResult.Authenticated(new AuthenticationResultBuilder()
            .With_AccessToken("authenticatedAccesstoken")
            .With_UniqueId("authenticatedUserId").Build());

        [SetUp]
        public void Setup()
        {
            Cleanup();
            MockLogService = new MockBuilder<ILogService>();
            MockTimeService = new MockBuilder<ITimeService>();
            MockMsalClientProxy = new MockBuilder<IMsalClientProxy>();
            MockIocService = new MockBuilder<IIocService>();
            MockAuthenticatedAccessToken = new MockBuilder<IAuthenticatedAccessToken>();

            CancelToken = new CancellationTokenSource().Token;
            
            MockMsalClientProxy.Mock.Setup(x => x.AuthenticateSilentlyAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(AuthenticatedUserResult);

            MockIocService.Mock.Setup(x => x.Resolve<IMsalClientProxy>())
                .Returns(MockMsalClientProxy.Object);
        }

        protected override AzureAuthenticationClientService ConstructSut()
        {
            return new AzureAuthenticationClientService(
                MockLogService.Object,
                MockIocService.Object,
                MockTimeService.Object,
                MockAuthenticatedAccessToken.Object);
        }
    }
}