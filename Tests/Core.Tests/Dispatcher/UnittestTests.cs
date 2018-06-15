using System;
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
    }
}
