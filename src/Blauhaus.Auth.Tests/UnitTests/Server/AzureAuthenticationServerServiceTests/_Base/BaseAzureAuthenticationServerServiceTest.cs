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
        
        protected MockBuilder<IServiceProvider> MockServiceProvider= null!;
        internal MockBuilder<IAdalAuthenticationContextProxy> MockAdalAuthenticationContext= null!;
        internal MockBuilder<IAzureActiveDirectoryServerConfig> MockAzureActiveDirectoryServerConfig= null!;
        internal MockBuilder<IHttpClientService> MockHttpClientService = null!;
        protected AnalyticsLoggerMockBuilder<AzureAuthenticationServerService> MockLogger = null!;
        
        
        [SetUp]
        public virtual void Setup()
        {
            Cleanup();
            MockAdalAuthenticationContext = new MockBuilder<IAdalAuthenticationContextProxy>();
            MockServiceProvider = new MockBuilder<IServiceProvider>();
            MockAzureActiveDirectoryServerConfig = new MockBuilder<IAzureActiveDirectoryServerConfig>();
            MockHttpClientService = new MockBuilder<IHttpClientService>();
            MockLogger = new AnalyticsLoggerMockBuilder<AzureAuthenticationServerService>();

            MockServiceProvider.Mock.Setup(x => x.GetService(typeof(IAdalAuthenticationContextProxy)))
                .Returns(MockAdalAuthenticationContext.Object);
        }

        protected override AzureAuthenticationServerService ConstructSut()
        {
            return new AzureAuthenticationServerService(
                MockHttpClientService.Object,
                MockAzureActiveDirectoryServerConfig.Object,
                MockAdalAuthenticationContext.Object,
                MockLogger.Object);
        }
    }
}