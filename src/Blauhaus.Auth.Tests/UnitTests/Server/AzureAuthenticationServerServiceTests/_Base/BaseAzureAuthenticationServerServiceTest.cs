using System;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Server.Azure.AdalProxy;
using Blauhaus.Auth.Server.Azure.Config;
using Blauhaus.Auth.Server.Azure.Service;
using Blauhaus.Auth.Server.Azure.User;
using Blauhaus.Common.TestHelpers;
using Blauhaus.HttpClientService.Abstractions;
using Blauhaus.Ioc.Abstractions;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Server.AzureAuthenticationServerServiceTests._Base
{
    public class BaseAzureAuthenticationServerServiceTest : BaseUnitTest<AzureAuthenticationServerService<IAzureActiveDirectoryUser>>
    {
        
        protected MockBuilder<IServiceProvider> MockServiceProvider;
        internal MockBuilder<IAdalAuthenticationContextProxy> MockAdalAuthenticationContext;
        internal MockBuilder<IAzureActiveDirectoryServerConfig> MockAzureActiveDirectoryServerConfig;
        internal MockBuilder<IHttpClientService> MockHttpClientService;
        internal MockBuilder<IAnalyticsService> MockAnalyticsService;


        [SetUp]
        public virtual void Setup()
        {
            Cleanup();
            MockAdalAuthenticationContext = new MockBuilder<IAdalAuthenticationContextProxy>();
            MockServiceProvider = new MockBuilder<IServiceProvider>();
            MockAzureActiveDirectoryServerConfig = new MockBuilder<IAzureActiveDirectoryServerConfig>();
            MockHttpClientService = new MockBuilder<IHttpClientService>();
            MockAnalyticsService = new MockBuilder<IAnalyticsService>();

            MockServiceProvider.Mock.Setup(x => x.GetService(typeof(IAdalAuthenticationContextProxy)))
                .Returns(MockAdalAuthenticationContext.Object);
        }

        protected override AzureAuthenticationServerService<IAzureActiveDirectoryUser> ConstructSut()
        {
            return new AzureAuthenticationServerService<IAzureActiveDirectoryUser>(
                MockHttpClientService.Object,
                MockAzureActiveDirectoryServerConfig.Object,
                MockServiceProvider.Object,
                MockAnalyticsService.Object);
        }
    }
}