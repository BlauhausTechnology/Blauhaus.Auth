using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Analytics.TestHelpers.MockBuilders;
using Blauhaus.TestHelpers.BaseTests;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Base
{
    public abstract class BaseAuthTest<TSut> : BaseServiceTest<TSut> where TSut : class
    {

        [SetUp]
        public virtual void Setup()
        {
            Cleanup();

            AddService(x => MockLogger.Object);
        }

        protected AnalyticsLoggerMockBuilder<TSut> MockLogger
            => AddMock<AnalyticsLoggerMockBuilder<TSut>, IAnalyticsLogger<TSut>>().Invoke();

    }
}