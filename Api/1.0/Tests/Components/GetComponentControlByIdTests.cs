using System;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.Components
{
    public class GetComponentControlByIdTests
    {
        /// <summary>
        /// Тест проверяет получение контрола компонента по Id
        /// </summary>
        [Fact]
        public void GetComponentByIdTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var root = client.GetRootComponentControl();

            // создадим новый
            var message = TestHelper.GetRandomGetOrCreateComponentData(client);
            message.Version = "1.0";
            var component = root.GetOrCreateChildComponentControl(message);

            // получим по Id
            var existingComponent = client.GetComponentControl(component.Info.Id);
            TestHelper.CheckComponent(message, existingComponent);

            // проверим, что ссылки контролов разные (НЕ кэшируется)
            var existingComponent2 = client.GetComponentControl(component.Info.Id);
            TestHelper.CheckComponent(message, existingComponent2);
            Assert.False(ReferenceEquals(existingComponent, existingComponent2));
        }

        /// <summary>
        /// Тест проверяет получение контрола компонента по несуществующему Id
        /// </summary>
        [Fact]
        public void GetComponentByWrongIdTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();

            // получим по несуществующему Id
            var existingComponent = client.GetComponentControl(Guid.Empty);
            Assert.True(existingComponent.IsFake());
        }
    }
}
