using System;
using Zidium.Api.Dto;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.Components
{
    public class GetOrCreateComponentTests
    {
        /// <summary>
        /// Тест проверяет, что повторное создание компонента не создает дубль
        /// </summary>
        [Fact]
        public void SecondCreateTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var root = client.GetRootComponentControl();

            // создадим новый
            var message = TestHelper.GetRandomGetOrCreateComponentData(client);
            var component = root.GetOrCreateChildComponentControl(message);
            TestHelper.CheckComponent(message, component);

            // получаем существующий (без обновления)
            var component2 = root.GetOrCreateChildComponentControl(message.ComponentTypeControl.SystemName, message.SystemName);
            var component3 = root.GetOrCreateChildComponentControl(message);
            TestHelper.CheckComponent(component, component2);
            TestHelper.CheckComponent(component, component3);
            Assert.False(ReferenceEquals(component, component2));
            Assert.False(ReferenceEquals(component, component3));
        }

        /// <summary>
        /// Тест проверяет, что GetOrCreateChildComponentControl изменяет только версию существующего компонента
        /// </summary>
        [Fact]
        public void VersionTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var root = client.GetRootComponentControl();

            // создадим новый
            var message = TestHelper.GetRandomGetOrCreateComponentData(client);
            message.Version = "1.0";
            var component = root.GetOrCreateChildComponentControl(message);
            TestHelper.CheckComponent(message, component);

            // получаем существующий (без обновления = обновляется только версия !!!)
            account = TestHelper.GetTestAccount();
            client = account.GetClient();
            root = client.GetRootComponentControl();
            message.Version = "2.0";
            message.DisplayName = Guid.NewGuid().ToString();
            var component2 = root.GetOrCreateChildComponentControl(message);

            Assert.False(component2.IsFake());
            Assert.Equal(component.Info.Id, component2.Info.Id);
            Assert.Equal("2.0", component2.Version); // изменилась
            Assert.Equal(component.Info.DisplayName, component2.Info.DisplayName); // не изменился
        }

        /// <summary>
        /// Тест проверяет, что ParentComponentId можно не указывать
        /// </summary>
        [Fact]
        public void NullParentComponentId()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var dtoService = TestHelper.GetDtoService(client.ApiService);
            var root = client.ApiService.GetRootComponent();
            root.Check();
            var type = client.GetOrCreateComponentTypeControl("testType");
            Assert.False(type.IsFake());
            var request = new GetOrCreateComponentRequestDto()
            {
                Token = new AccessTokenDto()
                {
                    SecretKey = client.AccessToken.SecretKey
                },
                Data = new GetOrCreateComponentRequestDtoData()
                {
                    SystemName = "test",
                    ParentComponentId = null,
                    TypeId = type.Info.Id
                }
            };
            var response = dtoService.GetOrCreateComponent(request);
            response.Check();
            Assert.Equal(root.Data.Id, response.Data.Component.ParentId);
        }
    }
}
