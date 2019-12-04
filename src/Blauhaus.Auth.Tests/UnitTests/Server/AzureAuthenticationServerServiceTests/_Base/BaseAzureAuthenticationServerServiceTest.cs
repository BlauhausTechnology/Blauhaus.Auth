using System.Threading;
using Blauhaus.Auth.Abstractions.ClientAuthenticationHandlers;
using Blauhaus.Auth.Client.Azure.MsalProxy;
using Blauhaus.Auth.Server.Azure.AdalProxy;
using Blauhaus.Auth.Server.Azure.Config;
using Blauhaus.Auth.Server.Azure.Service;
using Blauhaus.Auth.Server.Azure.User;
using Blauhaus.Auth.Tests.Builders;
using Blauhaus.Common.TestHelpers;
using Blauhaus.Common.Time.Service;
using Blauhaus.HttpClientService.Abstractions;
using Blauhaus.HttpClientService.Service;
using Blauhaus.Ioc.Abstractions;
using Blauhaus.Loggers.Common.Abstractions;
using Microsoft.Identity.Client;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Server.AzureAuthenticationServerServiceTests._Base
{
    public class BaseAzureAuthenticationServerServiceTest : BaseUnitTest<AzureAuthenticationServerService<IAzureActiveDirectoryUser>>
    {
        
        protected MockBuilder<IIocService> MockIocService;
        internal MockBuilder<IAdalAuthenticationContextProxy> MockAdalAuthenticationContext;
        internal MockBuilder<IAzureActiveDirectoryServerConfig> MockAzureActiveDirectoryServerConfig;
        internal MockBuilder<IHttpClientService> MockHttpClientService;


        [SetUp]
        public virtual void Setup()
        {
            Cleanup();
            MockAdalAuthenticationContext = new MockBuilder<IAdalAuthenticationContextProxy>();
            MockIocService = new MockBuilder<IIocService>();
            MockAzureActiveDirectoryServerConfig = new MockBuilder<IAzureActiveDirectoryServerConfig>();
            MockHttpClientService = new MockBuilder<IHttpClientService>();

            MockIocService.Mock.Setup(x => x.Resolve<IAdalAuthenticationContextProxy>())
                .Returns(MockAdalAuthenticationContext.Object);
        }

        protected override AzureAuthenticationServerService<IAzureActiveDirectoryUser> ConstructSut()
        {
            return new AzureAuthenticationServerService<IAzureActiveDirectoryUser>(
                MockHttpClientService.Object,
                MockAzureActiveDirectoryServerConfig.Object,
                MockIocService.Object);
        }
    }
}