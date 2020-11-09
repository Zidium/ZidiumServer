using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.UnitTestTypes
{
    public class UnitTestTypeTests : BaseTest
    {
        [Fact]
        public void GetOrCreateTest()
        {
            // Создаем 1-ый раз
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var createData = TestHelper.GetRandomGetOrCreateUnitTestTypeData();
            var type = client.GetOrCreateUnitTestTypeControl(createData);
            TestHelper.CheckUnitTestType(createData, type);

            // Создаем 2-ой раз
            account = TestHelper.GetTestAccount();
            client = account.GetClient();
            var type2 = client.GetOrCreateUnitTestTypeControl(createData.SystemName);
            TestHelper.CheckUnitTestType(type, type2);
        }
    }
}
