using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Server.Azure.User;
using Blauhaus.Auth.Tests.UnitTests.Server.AzureAuthenticationServerServiceTests._Base;
using Blauhaus.Common.TestHelpers;
using Blauhaus.HttpClientService.Abstractions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Server.AzureAuthenticationServerServiceTests
{
    public class GetUserAsyncTests : BaseAzureAuthenticationServerServiceTest
    {
        private Guid _userObjectId;
        private readonly Dictionary<string, object> _serializedAzureUser = 
            JsonConvert.DeserializeObject<Dictionary<string, object>>("{\r\n  " +
                                                                      "\"objectType\": \"User\",\r\n  " +
                                                                      "\"objectId\": \"29d195eb-68d1-45a3-8183-5fd8b5a72c0c\",\r\n  " +
                                                                      "\"createdDateTime\": \"2019-11-06T15:39:23Z\",\r\n  " +
                                                                      "\"creationType\": \"LocalAccount\",\r\n  " +
                                                                      "\"displayName\": \"Adrian Frielinghaus\",\r\n  " +
                                                                      "\"givenName\": \"Adrian\",\r\n  " +
                                                                      "\"signInNames\": " +"   [\r\n    {\r\n      \"type\": \"emailAddress\",\r\n      \"value\": \"adrian@maxxor.com\"\r\n    }\r\n  ],\r\n " +
                                                                      " \"surname\": \"Frielinghaus\",\r\n  " +
                                                                      "\"userPrincipalName\": \"29d195eb-68d1-45a3-8183-5fd8b5a72c0c@minegameauth.onmicrosoft.com\",\r\n  " +
                                                                      "\"userType\": \"Member\",\r\n  " +
                                                                      "\"extension_b2ea915621b940d8ae234cbb3a776931_RoleLevel\": 120\r\n}\r\n");

        private readonly Dictionary<string, object> _serializedAzureUserWithNoEmail = 
            JsonConvert.DeserializeObject<Dictionary<string, object>>("{\r\n  " +
                                                                      "\"objectType\": \"User\",\r\n  " +
                                                                      "\"objectId\": \"29d195eb-68d1-45a3-8183-5fd8b5a72c0c\",\r\n  " +
                                                                      "\"createdDateTime\": \"2019-11-06T15:39:23Z\",\r\n  " +
                                                                      "\"creationType\": \"LocalAccount\",\r\n  " +
                                                                      "\"displayName\": \"Adrian Frielinghaus\",\r\n  " +
                                                                      "\"givenName\": \"Adrian\",\r\n  " +
                                                                      " \"surname\": \"Frielinghaus\",\r\n  " +
                                                                      "\"userPrincipalName\": \"29d195eb-68d1-45a3-8183-5fd8b5a72c0c@minegameauth.onmicrosoft.com\",\r\n  " +
                                                                      "\"userType\": \"Member\",\r\n  " +
                                                                      "\"extension_b2ea915621b940d8ae234cbb3a776931_RoleLevel\": 120\r\n}\r\n");

        private MockBuilder<IAzureActiveDirectoryUser> _mockUser;

        public override void Setup()
        {
            base.Setup();
            _userObjectId = Guid.Parse("29d195eb-68d1-45a3-8183-5fd8b5a72c0c");
            MockAzureActiveDirectoryServerConfig
                .With(x => x.ExtensionsApplicationId, "b2ea915621b940d8ae234cbb3a776931");
            MockHttpClientService.Mock.Setup(x => x.GetAsync<Dictionary<string, object>>(It.IsAny<IHttpRequestWrapper>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_serializedAzureUser);
            _mockUser = new MockBuilder<IAzureActiveDirectoryUser>();
            MockServiceProvider.Mock.Setup(x => x.GetService(typeof(IAzureActiveDirectoryUser)))
                .Returns(_mockUser.Object);
        }

        [Test]
        public async Task SHOULD_send_request_to_correct_endpoint_with_accesstoken()
        {
            //Arrange
            MockAdalAuthenticationContext.Mock.Setup(x => x.AcquireAccessTokenAsync())
                .ReturnsAsync("azureAccessToken");
            MockAzureActiveDirectoryServerConfig
                .With(x => x.GraphEndpoint, "https://graph.windows.net/")
                .With(x => x.TenantId, "minegameauth.onmicrosoft.com")
                .With(x => x.GraphVersion, "api-version=1.6");

            //Act
            await Sut.GetUserAsync(_userObjectId.ToString(), CancellationToken.None);

            //Assert
            MockHttpClientService.Mock.Verify(x => x.GetAsync<Dictionary<string, object>>(It.Is<IHttpRequestWrapper>(y =>
                y.Endpoint == "https://graph.windows.net/minegameauth.onmicrosoft.com/users/29d195eb-68d1-45a3-8183-5fd8b5a72c0c?api-version=1.6" &&
                y.AuthorizationHeader.Key == "Bearer" &&
                y.AuthorizationHeader.Value == "azureAccessToken"), CancellationToken.None));
        }

        [Test]
        public async Task SHOULD_initialize_and_return_user()
        {
            //Arrange
            MockHttpClientService.Mock.Setup(x => x.GetAsync<Dictionary<string, object>>(It.IsAny<IHttpRequestWrapper>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_serializedAzureUser);
            var user = new DefaultAzureActiveDirectoryUser();
            MockServiceProvider.Mock.Setup(x => x.GetService(typeof(IAzureActiveDirectoryUser)))
                .Returns(user);
            
            //Act
            var result = await Sut.GetUserAsync(_userObjectId.ToString(), CancellationToken.None);

            //Assert
            Assert.That(result.EmailAddress, Is.EqualTo("adrian@maxxor.com"));
            Assert.That(result.AuthenticatedUserId, Is.EqualTo("29d195eb-68d1-45a3-8183-5fd8b5a72c0c"));
        }

        
        [Test]
        public async Task SHOULD_log_operation_and_trace()
        {
            //Arrange
            MockHttpClientService.Mock.Setup(x => x.GetAsync<Dictionary<string, object>>(It.IsAny<IHttpRequestWrapper>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_serializedAzureUser);
            var user = new DefaultAzureActiveDirectoryUser();
            MockServiceProvider.Mock.Setup(x => x.GetService(typeof(IAzureActiveDirectoryUser)))
                .Returns(user);
            
            //Act
            await Sut.GetUserAsync(_userObjectId.ToString(), CancellationToken.None);

            //Assert
            MockAnalyticsService.Mock.Verify(x => x.ContinueOperation("Get user profile from Azure AD", It.Is<Dictionary<string, object>>(y => 
                (string) y["AuthenticatedUserId"] == _userObjectId.ToString())));
            MockAnalyticsService.Mock.Verify(x => x.Trace("User profile retrieved from Azure AD", 
                LogSeverity.Verbose, It.Is<Dictionary<string, object>>(y => y["AzureADUser"] == user)));
        }


        [Test]
        public async Task IF_Email_Address_is_not_valid_SHOULD_make_it_null()
        {
            //Arrange
            MockHttpClientService.Mock.Setup(x => x.GetAsync<Dictionary<string, object>>(It.IsAny<IHttpRequestWrapper>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_serializedAzureUserWithNoEmail);
            var user = new DefaultAzureActiveDirectoryUser();
            MockServiceProvider.Mock.Setup(x => x.GetService(typeof(IAzureActiveDirectoryUser)))
                .Returns(user);
            
            //Act
            var result = await Sut.GetUserAsync(_userObjectId.ToString(), CancellationToken.None);

            //Assert
            Assert.That(result.EmailAddress, Is.Null);
        }

    }
}