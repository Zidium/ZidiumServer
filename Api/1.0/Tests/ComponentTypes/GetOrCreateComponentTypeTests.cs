using System;
using Zidium.Api;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.ComponentTypes
{
    public class GetOrCreateComponentTypeTests : BaseTest
    {
        [Fact]
        public void GetOrCreateComponentTypeTest()
        {
            IClient client = TestHelper.GetTestAccount().GetClient();

            // создаем родителя
            var data = new GetOrCreateComponentTypeData("TestComponentType")
            {
                DisplayName = "test display name"
            };
            var response = client.ApiService.GetOrCreateComponentType(data);
            Assert.True(response.Success);
            Assert.NotEqual(response.Data.Id, Guid.Empty);
            Assert.Equal("TestComponentType", response.Data.SystemName);
            Assert.Equal("test display name", response.Data.DisplayName);
        }

        [Fact]
        public void GetOrCreateComponentTypeControlTest()
        {
            IClient client = TestHelper.GetTestAccount().GetClient();

            // создаем родителя
            var data = new GetOrCreateComponentTypeData("TestComponentType")
            {
                DisplayName = "test display name"
            };
            var control = client.GetOrCreateComponentTypeControl(data);
            Assert.False(control.IsFake());
            Assert.NotEqual(control.Info.Id, Guid.Empty);
            Assert.Equal("TestComponentType", control.Info.SystemName);
            Assert.Equal("test display name", control.Info.DisplayName);
        }

        [Fact]
        public void CommonComponentTypeTest()
        {
            IClient client = TestHelper.GetTestAccount().GetClient();

            // Проверим, что можно получить системный тип
            var type = client.GetOrCreateComponentTypeControl(SystemComponentType.WebSite.SystemName);
            Assert.False(type.IsFake());
            Assert.True(type.Info.IsSystem);

        }
    }
}
