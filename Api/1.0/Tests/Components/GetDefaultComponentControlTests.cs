using System;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.Components
{
    public class GetDefaultComponentControlTests : BaseTest
    {
        /// <summary>
        /// Тест проверяет получение дефолтного контрола компонента
        /// </summary>
        [Fact]
        public void GetSuccess()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var root = client.GetRootComponentControl();

            // создадим новый
            var message = TestHelper.GetRandomGetOrCreateComponentData(client);
            message.Version = "1.0";
            var component = root.GetOrCreateChildComponentControl(message);
            Assert.False(component.IsFake());

            // получим
            client.Config.DefaultComponent.Id = component.Info.Id;
            var defaultComponent = client.GetDefaultComponentControl();
            TestHelper.CheckComponent(message, defaultComponent);

            // проверим, что ссылки контролов одинаковые
            var defaultComponent2 = client.GetDefaultComponentControl();
            Assert.True(ReferenceEquals(defaultComponent, defaultComponent2));
        }

        /// <summary>
        /// Тест проверяет получение дефолтного контрола компонента по несуществующему Id
        /// </summary>
        [Fact]
        public void GetComponentByWrongId()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();

            // получим по несуществующему Id
            client.Config.DefaultComponent.Id = Guid.NewGuid();
            var defaultComponent = client.GetDefaultComponentControl();
            Assert.True(defaultComponent.IsFake());

            // получим еще раз
            var defaultComponent2 = client.GetDefaultComponentControl();
            Assert.True(defaultComponent2.IsFake());

            // ссылки одинаковые
            Assert.True(ReferenceEquals(defaultComponent, defaultComponent2));
        }


        /// <summary>
        /// Тест проверяет получение дефолтного контрола компонента, который удалили
        /// </summary>
        [Fact]
        public void GetDeleted()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var root = client.GetRootComponentControl();

            // создадим новый
            var message = TestHelper.GetRandomGetOrCreateComponentData(client);
            message.Version = "1.0";
            var component = root.GetOrCreateChildComponentControl(message);
            Assert.False(component.IsFake());

            // удалим
            component.Delete();

            // получим по несуществующему Id
            client.Config.DefaultComponent.Id = component.Info.Id;
            var defaultComponent = client.GetDefaultComponentControl();
            Assert.True(defaultComponent.IsFake());

            // получим еще раз
            var defaultComponent2 = client.GetDefaultComponentControl();
            Assert.True(defaultComponent2.IsFake());

            // ссылки одинаковые
            Assert.True(ReferenceEquals(defaultComponent, defaultComponent2));
        }

        /// <summary>
        /// Тест проверяет получение дефолтного контрола компонента, если его Id не указан в конфиге
        /// </summary>
        [Fact]
        public void GetComponentWithEmptyId()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();

            // получим без указания Id
            client.Config.DefaultComponent.Id = null;
            var defaultComponent = client.GetDefaultComponentControl();
            Assert.True(defaultComponent.IsFake());
        }
    }
}
