using System;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Dispatcher
{
    public class UnitTestTypeTests
    {
        [Fact]
        public void GetOrCreateUnitTestTypeTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = TestHelper.GetDispatcherClient();

            var data = new GetOrCreateUnitTestTypeRequestData()
            {
                SystemName = "UnitTestType." + Guid.NewGuid()
            };

            var response = dispatcher.GetOrCreateUnitTestType(account.Id, data);
            response.Check();

            var unitTestTypeId = response.Data.Id;

            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var unitTestTypeRepository = context.GetUnitTestTypeRepository();
                var unitTestType = unitTestTypeRepository.GetByIdOrNull(unitTestTypeId);
                Assert.NotNull(unitTestType);
                Assert.Equal(data.SystemName, unitTestType.SystemName);
            }

        }
    }
}
