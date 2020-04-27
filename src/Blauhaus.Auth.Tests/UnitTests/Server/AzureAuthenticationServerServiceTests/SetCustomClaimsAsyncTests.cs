using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Tests.UnitTests.Server.AzureAuthenticationServerServiceTests._Base;
using Blauhaus.HttpClientService.Abstractions;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Server.AzureAuthenticationServerServiceTests
{
    public class SetCustomClaimsAsyncTests : BaseAzureAuthenticationServerServiceTest
    {
        private Guid _userObjectId;

        public override void Setup()
        {
            base.Setup();
            _userObjectId = Guid.Parse("29d195eb-68d1-45a3-8183-5fd8b5a72c0c");
        }


        [Test]
        public async Task SHOULD_send_request_to_correct_endpoint()
        {
            //Arrange
            MockAzureActiveDirectoryServerConfig
                .With(x => x.GraphEndpoint, "https://graph.windows.net/")
                .With(x => x.TenantId, "minegameauth.onmicrosoft.com")
                .With(x => x.GraphVersion, "api-version=1.6");

            //Act
            await Sut.SetCustomClaimsAsync(_userObjectId, new Dictionary<string, string>
            {
                {"RoleLevel", "120" },
                {"LuckyNumber", "12" }
            }, CancellationToken.None);

            //Assert
            MockHttpClientService.Mock.Verify(x => x.PatchAsync<string>(It.Is<IHttpRequestWrapper<JObject>>(y =>
                y.Endpoint == "https://graph.windows.net/minegameauth.onmicrosoft.com/users/29d195eb-68d1-45a3-8183-5fd8b5a72c0c?api-version=1.6"), CancellationToken.None));
        }

        [Test]
        public async Task SHOULD_construct_custom_property_name_and_send_as_patch_request_body()
        {
            //Arrange
            MockAzureActiveDirectoryServerConfig
                .With(x => x.ExtensionsApplicationId, "b2ea915621b940d8ae234cbb3a776931");

            //Act
            await Sut.SetCustomClaimsAsync(_userObjectId, new Dictionary<string, string>
            {
                {"RoleLevel", "120" },
                {"LuckyNumber", "12" }
            }, CancellationToken.None);

            //Assert
            var expectedJson = new JObject
            {
                ["extension_b2ea915621b940d8ae234cbb3a776931_RoleLevel"] = "120",
                ["extension_b2ea915621b940d8ae234cbb3a776931_LuckyNumber"] = "12",
            };
            MockHttpClientService.Mock.Verify(x => x.PatchAsync<string>(It.Is<IHttpRequestWrapper<JObject>>(y =>
                y.Request.ToString() == expectedJson.ToString()
            ), CancellationToken.None));
        }

        [Test]
        public async Task SHOULD_add_azure_auth_token_header_from_ADAL_client()
        {
            //Arrange
            MockAdalAuthenticationContext.Mock.Setup(x => x.AcquireAccessTokenAsync())
                .ReturnsAsync("azureAccessToken");

            //Act
            await Sut.SetCustomClaimsAsync(_userObjectId, new Dictionary<string, string>
            {
                {"RoleLevel", "120" },
                {"LuckyNumber", "12" }
            }, CancellationToken.None);

            //Assert
            MockHttpClientService.Mock.Verify(x => x.PatchAsync<string>(It.Is<IHttpRequestWrapper<JObject>>(y =>
                y.AuthorizationHeader.Key == "Bearer" &&
                y.AuthorizationHeader.Value == "azureAccessToken"), CancellationToken.None));
        }
        
        [Test]
        public async Task SHOULD_trace_success()
        {
            //Arrange
            MockAzureActiveDirectoryServerConfig
                .With(x => x.ExtensionsApplicationId, "b2ea915621b940d8ae234cbb3a776931");
            MockAdalAuthenticationContext.Mock.Setup(x => x.AcquireAccessTokenAsync())
                .ReturnsAsync("azureAccessToken");

            //Act
            await Sut.SetCustomClaimsAsync(_userObjectId, new Dictionary<string, string>
            {
                {"RoleLevel", "120" },
                {"LuckyNumber", "12" }
            }, CancellationToken.None);

            //Assert
            var expectedJson = new JObject
            {
                ["extension_b2ea915621b940d8ae234cbb3a776931_RoleLevel"] = "120",
                ["extension_b2ea915621b940d8ae234cbb3a776931_LuckyNumber"] = "12",
            };
            MockAnalyticsService.Mock.Verify(x => x.Trace(Sut, "Custom claims set", LogSeverity.Information, It.Is<Dictionary<string, object>>(y =>
                (string)y["RoleLevel"] == "120" &&
                (string)y["LuckyNumber"] == "12" &&
                (Guid)y["UserId"] == _userObjectId &&
                y["Json"].ToString() == expectedJson.ToString()), It.IsAny<string>()));
        }
    }
}