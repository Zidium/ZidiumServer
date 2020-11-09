using System;
using Zidium.Api;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.ComponentTypes
{
    public class GetOrCreateComponentTypeControlTests : BaseTest
    {
        /// <summary>
        /// Простая проверка того, что запрос успешно отрабатывает
        /// </summary>
        [Fact]
        public void OnlineTest()
        {
            IClient client = TestHelper.GetTestAccount().GetClient();
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

        /// <summary>
        /// Проверим, что контролы создаются даже при выключенном интернете + загружаются когда он есть
        /// </summary>
        [Fact]
        public void OfflineTest()
        {
            IClient client = TestHelper.GetTestAccount().GetClient();
            client.Config.Access.WaitOnError = TimeSpan.FromMinutes(10);
            var offlineService = new FakeApiService();
            var onlineService = client.ApiService;
            var data = new GetOrCreateComponentTypeData("TestComponentType")
            {
                DisplayName = "test display name"
            };

            // интернет включен
            var control1 = client.GetOrCreateComponentTypeControl(data);
            var control2 = client.GetOrCreateComponentTypeControl(data);
            Assert.False(control1.IsFake()); // чтобы заставить выполнить запрос

            // интернет выключен
            client.SetApiService(offlineService);
            var control3 = client.GetOrCreateComponentTypeControl(data);

            // проверим
            Assert.False(control1.IsFake()); // потому что загрузил свои данные когда был интернет
            Assert.True(control2.IsFake()); // потому что независимая обертка
            Assert.True(control3.IsFake());
            
            Assert.False(object.ReferenceEquals(control1, control2));
            Assert.False(object.ReferenceEquals(control1, control3));

            var control4 = client.GetOrCreateComponentTypeControl("test");
            Assert.True(control4.IsFake()); // интернета нет
            client.SetApiService(onlineService);
            client.Config.Access.WaitOnError = TimeSpan.Zero;
            Assert.False(control3.IsFake()); // интернет есть
            Assert.False(control4.IsFake()); // интернет есть
        }

        /// <summary>
        /// Тест проверяет, что для системного типа не нужен интернет, чтобы получить настоящий контрол папки
        /// </summary>
        [Fact]
        public void FolderTest()
        {
            IClient client = TestHelper.GetTestAccount().GetClient();
            client.SetApiService(new FakeApiService()); // выключим интернет
            var control = client.GetFolderComponentTypeControl();
            Assert.False(control.IsFake());
            Assert.NotNull(control.Info);
            Assert.NotEqual(control.Info.Id, Guid.Empty);
            Assert.Equal("System.ComponentTypes.Folder", control.Info.SystemName);
            Assert.Equal(new Guid("05d8d8e8-1050-41d3-b98e-c17eb22378b9"), control.Info.Id);
        }
    }
}
