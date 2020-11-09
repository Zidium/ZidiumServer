using System;
using Zidium.Api;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.Components
{
    public class ControlWrapperTests : BaseTest
    {
        /// <summary>
        /// Тест проверяет, что при выключенном интернете контролы фейковые, а при включеном становятся нормальными
        /// </summary>
        [Fact]
        public void Test1()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            client.Config.Access.WaitOnError = TimeSpan.Zero;
            var onlineService = client.ApiService;

            // выключим интернет
            var offlineService = new FakeApiService();
            client.SetApiService(offlineService);

            var root = client.GetRootComponentControl();
            Assert.True(root.IsFake());
            Assert.Null(root.Info);
            Assert.NotNull(root.Log);

            var getOrCreateData = TestHelper.GetRandomGetOrCreateComponentData(client);
            var child = root.GetOrCreateChildComponentControl(getOrCreateData);
            Assert.True(child.IsFake());
            Assert.Null(child.Info);

            // включим интернет
            client.SetApiService(onlineService);

            Assert.False(root.IsFake());
            Assert.NotNull(root.Info);

            Assert.False(child.IsFake());
            Assert.NotNull(child.Info);
        }

        /// <summary>
        /// Тест проверяет, что ссылки на контроль компонента НЕ кэшируются клиентом
        /// </summary>
        [Fact]
        public void TestReferenceEquals()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var root = client.GetRootComponentControl();
            
            // root кэшируется, остальные контролы нет
            Assert.False(root.IsFake());
            var root2 = client.GetRootComponentControl();
            Assert.True(ReferenceEquals(root, root2));
            var lastResponseDate = client.LastResponse.Date;

            // получим ссылки на компоненты (загрузки нет)
            var component1 = root.GetOrCreateChildComponentControl("type", "name");
            var component2 = root.GetOrCreateChildComponentControl("type", "name");
            Assert.Equal(lastResponseDate, client.LastResponse.Date);
            Assert.False(ReferenceEquals(component1, component2));

            // загрузим component1
            Assert.False(component1.IsFake());
            Assert.NotEqual(lastResponseDate, client.LastResponse.Date);
            lastResponseDate = client.LastResponse.Date;

            // получим component2 НЕ из кэша
            Assert.False(component2.IsFake());
            Assert.False(lastResponseDate == client.LastResponse.Date);
        }
    }
}
