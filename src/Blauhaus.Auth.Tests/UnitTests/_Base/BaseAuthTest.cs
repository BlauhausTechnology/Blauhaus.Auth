using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Analytics.TestHelpers;
using Blauhaus.TestHelpers.BaseTests;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests._Base
{
    public abstract class BaseAuthTest<TSut> : BaseServiceTest<TSut> where TSut : class
    {

        [SetUp]
        public virtual void Setup()
        {
            Cleanup();

            AddService(x => MockAnalyticsService.Object);
        }
        

        protected AnalyticsServiceMockBuilder MockAnalyticsService => AddMock<AnalyticsServiceMockBuilder, IAnalyticsService>().Invoke();

    }
}