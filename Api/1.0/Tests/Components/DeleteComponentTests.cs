using System.Linq;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.Components
{
    public class DeleteComponentTests
    {
        [Fact]
        public void DeleteComponentTest()
        {
            var account = TestHelper.GetTestAccount();
            var root = account.GetClient().GetRootComponentControl();

            // �������� ����� ��������� � ������ ���
            var component = account.CreateRandomComponentControl();
            Assert.False(component.IsFake());
            component.Delete();

            // ������� ��������� �� ��� �����
            var newComponent = root.GetOrCreateChildComponentControl(component.Info.Type.SystemName, component.Info.SystemName);
            Assert.False(newComponent.IsFake());

            // ��������, ��� ��� ����� ���������, � ������ Id
            Assert.NotEqual(component.Info.Id, newComponent.Info.Id);

            // ��������, ��� � ������ �������� ��������� ���
            var childs = root.GetChildComponents().Data;
            Assert.Empty(childs.Where(t => t.Id == component.Info.Id));
            Assert.NotEmpty(childs.Where(t => t.Id == newComponent.Info.Id));

        }
    }
}
