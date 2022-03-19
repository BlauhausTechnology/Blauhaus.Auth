using System;
using System.Collections.Generic;
using System.Threading;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.TestHelpers.MockBuilders;
using Blauhaus.Auth.Abstractions.AccessToken;
using Blauhaus.Auth.Client.Azure.Config;
using Blauhaus.Auth.Client.Azure.MsalProxy;
using Blauhaus.Auth.Client.Azure.Service;
using Blauhaus.Auth.Tests.Builders;
using Blauhaus.Ioc.Abstractions;
using Blauhaus.TestHelpers.BaseTests;
using Blauhaus.TestHelpers.MockBuilders;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Client.AzureAuthenticationClientServiceTests.Base
{
    public class BaseAuthenticationClientServiceTest : BaseUnitTest<AzureAuthenticationClientService>
    {
 
        protected const string AccessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ilg1ZVhrNHh5b2pORnVtMWtsMll0djhkbE5QNC1jNTdkTzZRR1RWQndhTmsifQ.eyJpc3MiOiJodHRwczovL21pbmVnYW1lYXV0aC5iMmNsb2dpbi5jb20vNTUwNzM3NDItMTA0ZC00N2FiLWJiYzQtNDA2ZjE3MmJjMmViL3YyLjAvIiwiZXhwIjoxNTg0MTkwMDY5LCJuYmYiOjE1ODQxODY0NjksImF1ZCI6ImVhNjdlMGYzLTVjNWUtNGM2YS04OWYwLWM4YjA5Njc5NmU5OSIsIm9pZCI6IjI5ZDE5NWViLTY4ZDEtNDVhMy04MTgzLTVmZDhiNWE3MmMwYyIsInN1YiI6IjI5ZDE5NWViLTY4ZDEtNDVhMy04MTgzLTVmZDhiNWE3MmMwYyIsImV4dGVuc2lvbl9Vc2VySWQiOiI5YzM2NDhjNC1mZGEzLTQzOGMtOGUzZS1kNWI5ODcwYTQzMmYiLCJleHRlbnNpb25fVXNlclR5cGUiOiJBZG1pbiIsImVtYWlscyI6WyJhZHJpYW5AbWF4eG9yLmNvbSJdLCJ0ZnAiOiJCMkNfMV9NaW5lR2FtZV9TaWduVXBfb3JfU2lnbkluIiwic2NwIjoicmVhZC5hbmQud3JpdGUiLCJhenAiOiJlYTY3ZTBmMy01YzVlLTRjNmEtODlmMC1jOGIwOTY3OTZlOTkiLCJ2ZXIiOiIxLjAiLCJpYXQiOjE1ODQxODY0Njl9.YTbnQivBMobOJzK_9NNxiAl223I22r_vkm2o9CdTwifbFig-gR3t9wKAJcxavfs-wzGwxdSGll8Zq1aYvOjQSZxD_K0VUqpboqXyrE6JZfNl8-35UopQXMvSkVhE6WyEljA8Y06RrOliDNoacKjmvKSzxIUIde3-UBlfKYveh84azQOns-GFhCZQhifEZRnpkT-1N1Y0ZS_PFjpxc3NOJMj3IZKvNOpF0Z_8q-wqo_Mrqh-DHeFkZbWZboYb0EjMXVPE2l_WiNNqdP7SpFEEcap8Zw7Dysyz27JzepWG0wXjFDOBA44vlORoW26i42rOwvfu64qhfvIxF4rsqSEBGw";
        protected static Guid UserId = Guid.Parse("29d195eb-68d1-45a3-8183-5fd8b5a72c0c");

        protected AnalyticsLoggerMockBuilder<AzureAuthenticationClientService> MockLogger => AddMock<AnalyticsLoggerMockBuilder<AzureAuthenticationClientService>, IAnalyticsLogger<AzureAuthenticationClientService>>().Invoke();
        protected MockBuilder<IIocService> MockIocService = null!;
        internal MsalClientProxyMockBuilder MockMsalClientProxy = null!;
        internal MockBuilder<IAuthenticatedAccessToken> MockAuthenticatedAccessToken = null!;
        internal MockBuilder<IAzureActiveDirectoryClientConfig> MockConfig = null!;
        protected CancellationToken MockCancelToken;

        protected readonly MsalClientResult MockAuthenticatedUserResult = MsalClientResult.Authenticated(new AuthenticationResultBuilder()
            .With_AccessToken(AccessToken)
            .With_UniqueId(UserId.ToString()).Build());



        [SetUp]
        public virtual void Setup()
        {
            Cleanup();
            MockMsalClientProxy = new MsalClientProxyMockBuilder();
            MockAuthenticatedAccessToken = new MockBuilder<IAuthenticatedAccessToken>();
            MockIocService = new MockBuilder<IIocService>();
            MockConfig = new MockBuilder<IAzureActiveDirectoryClientConfig>();
            MockCancelToken = new CancellationTokenSource().Token;
            
            MockMsalClientProxy.Where_AuthenticateSilentlyAsync_returns(MockAuthenticatedUserResult);

            MockIocService.Mock.Setup(x => x.Resolve<IMsalClientProxy>())
                .Returns(MockMsalClientProxy.Object);
        }

        protected override AzureAuthenticationClientService ConstructSut()
        {
            return new AzureAuthenticationClientService(
                MockLogger.Object,
                MockMsalClientProxy.Object,
                MockAuthenticatedAccessToken.Object, 
                MockConfig.Object);
        }
    }
}