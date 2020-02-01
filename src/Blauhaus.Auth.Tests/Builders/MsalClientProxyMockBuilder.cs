using System;
using System.Threading;
using Blauhaus.Auth.Client.Azure.MsalProxy;
using Blauhaus.Common.TestHelpers;
using Moq;

namespace Blauhaus.Auth.Tests.Builders
{
    internal class MsalClientProxyMockBuilder : BaseMockBuilder<MsalClientProxyMockBuilder, IMsalClientProxy>
    {
        public MsalClientProxyMockBuilder()
        {
        }

        public MsalClientProxyMockBuilder Where_AuthenticateSilentlyAsync_returns(MsalClientResult result)
        {
            Mock.Setup(x => x.AuthenticateSilentlyAsync(It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(result);
            return this;
        }

        public MsalClientProxyMockBuilder Where_AuthenticateSilentlyAsync_throws(Exception exception)
        {
            Mock.Setup(x => x.AuthenticateSilentlyAsync(It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ThrowsAsync(exception);
            return this;
        }

        public MsalClientProxyMockBuilder Where_LoginAsync_returns(MsalClientResult result)
        {
            Mock.Setup(x => x.LoginAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            return this;
        }

        public MsalClientProxyMockBuilder Where_LoginAsync_throws(Exception exception)
        {
            Mock.Setup(x => x.LoginAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
            return this;
        }


        public MsalClientProxyMockBuilder Where_ResetPasswordAsync_returns(MsalClientResult result)
        {
            Mock.Setup(x => x.ResetPasswordAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            return this;
        }

        public MsalClientProxyMockBuilder Where_ResetPasswordAsync_throws(Exception exception)
        {
            Mock.Setup(x => x.ResetPasswordAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
            return this;
        }

    }
}
