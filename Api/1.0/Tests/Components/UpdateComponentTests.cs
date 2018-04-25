using System;
using Xunit;
using Zidium.Api;
using Zidium.TestTools;

namespace ApiTests_1._0.Components
{
    public class UpdateComponentTests
    {
        /// <summary>
        /// Тест проверяет обновление всех свойств компонента
        /// </summary>
        [Fact]
        public void UpdateComponentTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var root = client.GetRootComponentControl();

            // создадим новый компонент
            var createComponentData = TestHelper.GetRandomGetOrCreateComponentData(client);
            var component = root.GetOrCreateChildComponentControl(createComponentData);
            Assert.False(component.IsFake());

            // создадим новый тип компонента
            var newComponentType = TestHelper.GetRandomComponentTypeControl(client);
            Assert.False(newComponentType.IsFake());

            // создадим нового родителя
            var createParentComponentData = TestHelper.GetRandomGetOrCreateComponentData(client);
            var parentComponent = root.GetOrCreateChildComponentControl(createParentComponentData);

            // обновим свойства компонента
            var updateComponentData = new UpdateComponentData()
            {
                SystemName = Guid.NewGuid().ToString(),
                DisplayName = Guid.NewGuid().ToString(),
                ParentId = parentComponent.Info.Id,
                TypeId = newComponentType.Info.Id,
                Version = "1.2.3.4"
            };
            updateComponentData.Properties.Set("Prop", "Value");

            var response = component.Update(updateComponentData);
            response.Check();

            // проверим новые значения свойств компонента
            var updatedComponent = client.GetComponentControl(component.Info.Id);

            Assert.False(updatedComponent.IsFake());
            Assert.Equal(updateComponentData.SystemName, updatedComponent.Info.SystemName);
            Assert.Equal(updateComponentData.DisplayName, updatedComponent.Info.DisplayName);
            Assert.Equal(updateComponentData.ParentId, updatedComponent.Info.ParentId);
            Assert.Equal(updateComponentData.TypeId, updatedComponent.Info.Type.Id);
            Assert.Equal(updateComponentData.Version, updatedComponent.Info.Version);

            var prop = updatedComponent.Info.Properties["Prop"];
            Assert.NotNull(prop);
            Assert.Equal("Value", prop.Value);
        }

        /// <summary>
        /// Тест проверяет, что пустой запрос на обновление ничего не меняет
        /// </summary>
        [Fact]
        public void UpdateComponentNoChangesTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var root = client.GetRootComponentControl();

            // создадим новый компонент
            var createComponentData = TestHelper.GetRandomGetOrCreateComponentData(client);
            var component = root.GetOrCreateChildComponentControl(createComponentData);
            Assert.False(component.IsFake());

            // обновим свойства компонента
            var updateComponentData = new UpdateComponentData()
            {
                Properties = null
            };

            var response = component.Update(updateComponentData);
            response.Check();

            // проверим новые значения свойств компонента
            var updatedComponent = client.GetComponentControl(component.Info.Id);

            Assert.False(updatedComponent.IsFake());
            Assert.Equal(component.Info.SystemName, updatedComponent.Info.SystemName);
            Assert.Equal(component.Info.DisplayName, updatedComponent.Info.DisplayName);
            Assert.Equal(component.Info.ParentId, updatedComponent.Info.ParentId);
            Assert.Equal(component.Info.Type.Id, updatedComponent.Info.Type.Id);
            Assert.Equal(component.Info.Version, updatedComponent.Info.Version);
            Assert.Equal(component.Info.Properties.Count, updatedComponent.Info.Properties.Count);

        }
    }

}
