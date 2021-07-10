using System.Linq;
using Xunit;
using Zidium.TestTools;

namespace Zidium.Api.Tests.Components
{
    public class DeleteComponentTests : BaseTest
    {
        [Fact]
        public void DeleteComponentTest()
        {
            var account = TestHelper.GetTestAccount();
            var root = account.GetClient().GetRootComponentControl();

            // Создадим новый компонент и удалим его
            var component = account.CreateRandomComponentControl();
            Assert.False(component.IsFake());
            component.Delete();

            // Получим компонент по его имени
            var newComponent = root.GetOrCreateChildComponentControl(component.Info.Type.SystemName, component.Info.SystemName);
            Assert.False(newComponent.IsFake());

            // Проверим, что это новый компонент, с другим Id
            Assert.NotEqual(component.Info.Id, newComponent.Info.Id);

            // Проверим, что в списке дочерних удалённого нет
            var childs = root.GetChildComponents().GetDataAndCheck();
            Assert.Empty(childs.Where(t => t.Id == component.Info.Id));
            Assert.NotEmpty(childs.Where(t => t.Id == newComponent.Info.Id));

        }
    }
}
