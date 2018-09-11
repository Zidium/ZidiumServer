using System;
using System.Linq;
using Xunit;
using Zidium.Core.Api;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Dispatcher
{
    public class UnittestTests
    {
        [Fact]
        public void CreateUnittestWithoutTypeTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = TestHelper.GetDispatcherClient();
            var component = account.CreateTestApplicationComponent();

            var data = new GetOrCreateUnitTestRequestData()
            {
                ComponentId = component.Id,
                UnitTestTypeId = null,
                SystemName = "Unittest." + Guid.NewGuid()
            };

            var unittest = dispatcher.GetOrCreateUnitTest(account.Id, data).Data;
            var unittestType = dispatcher.GetUnitTestTypeById(account.Id, unittest.TypeId).Data;
            Assert.Equal("CustomUnitTestType", unittestType.SystemName);
        }

        [Fact]
        public void SendUnitTestResultsTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = TestHelper.GetDispatcherClient();
            var component = account.CreateTestApplicationComponent();
            var unittest1 = TestHelper.CreateTestUnitTest(account.Id, component.Id);
            var unittest2 = TestHelper.CreateTestUnitTest(account.Id, component.Id);

            var data = new[]
            {
                new SendUnitTestResultRequestData()
                {
                    UnitTestId = unittest1.Id,
                    Result = UnitTestResult.Success
                },
                new SendUnitTestResultRequestData()
                {
                    UnitTestId = unittest2.Id,
                    Result = UnitTestResult.Warning
                }
            };

            dispatcher.SendUnitTestResults(account.Id, data);

            var status1 = dispatcher.GetUnitTestState(account.Id, component.Id, unittest1.Id).Data.Status;
            var status2 = dispatcher.GetUnitTestState(account.Id, component.Id, unittest2.Id).Data.Status;

            Assert.Equal(MonitoringStatus.Success, status1);
            Assert.Equal(MonitoringStatus.Warning, status2);
        }
    }
}
