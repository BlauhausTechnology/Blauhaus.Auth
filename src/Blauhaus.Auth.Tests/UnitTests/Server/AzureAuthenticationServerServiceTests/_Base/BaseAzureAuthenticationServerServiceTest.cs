using System;
using Blauhaus.Analytics.TestHelpers.MockBuilders;
using Blauhaus.Auth.Server.Azure.AdalProxy;
using Blauhaus.Auth.Server.Azure.Config;
using Blauhaus.Auth.Server.Azure.Service;
using Blauhaus.HttpClientService.Abstractions;
using Blauhaus.TestHelpers.BaseTests;
using Blauhaus.TestHelpers.MockBuilders;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Server.AzureAuthenticationServerServiceTests._Base
{
    public class BaseAzureAuthenticationServerServiceTest : BaseUnitTest<AzureAuthenticationServerService>
    {
        
        protected MockBuilder<IServiceProvider> MockServiceProvider;
        internal MockBuilder<IAdalAuthenticationContextProxy> MockAdalAuthenticationContext;
        internal MockBuilder<IAzureActiveDirectoryServerConfig> MockAzureActiveDirectoryServerConfig;
        internal MockBuilder<IHttpClientService> MockHttpClientService;
        internal AnalyticsServiceMockBuilder MockAnalyticsService;

        [SetUp]
        public virtual void Setup()
        {
            Cleanup();
            MockAdalAuthenticationContext = new MockBuilder<IAdalAuthenticationContextProxy>();
            MockServiceProvider = new MockBuilder<IServiceProvider>();
            MockAzureActiveDirectoryServerConfig = new MockBuilder<IAzureActiveDirectoryServerConfig>();
            MockHttpClientService = new MockBuilder<IHttpClientService>();
            MockAnalyticsService = new AnalyticsServiceMockBuilder();

            MockServiceProvider.Mock.Setup(x => x.GetService(typeof(IAdalAuthenticationContextProxy)))
                .Returns(MockAdalAuthenticationContext.Object);
        }

        protected override AzureAuthenticationServerService ConstructSut()
        {
            return new AzureAuthenticationServerService(
                MockHttpClientService.Object,
                MockAzureActiveDirectoryServerConfig.Object,
                MockAdalAuthenticationContext.Object,
                MockAnalyticsService.Object);
        }
    }
}