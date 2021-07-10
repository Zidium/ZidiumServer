using System.Linq;
using Xunit;
using Zidium.TestTools;

namespace Zidium.Api.Tests.Components
{
    public class ChildComponentsTests : BaseTest
    {
        /// <summary>
        /// Проверяет, что 3 созданных компонента успешно читаются из БД
        /// </summary>
        [Fact]
        public void GetChildComponentsTest()
        {
            var account = TestHelper.GetTestAccount();
            var root = account.GetClient().GetRootComponentControl();

            var component1 = account.CreateRandomComponentControl();
            var component2 = account.CreateRandomComponentControl();
            var component3 = account.CreateRandomComponentControl();

            var components = new[] { component1, component2, component3 };
            foreach (var component in components)
            {
                Assert.False(component.IsFake());
            }

            // проверим количество
            var childs = root.GetChildComponents().GetDataAndCheck();
            Assert.Equal(3, childs.Count(t => components.Any(x => x.Info.Id == t.Id)));

            // проверим
            foreach (var component in components)
            {
                var child = childs.Single(x => x.Id == component.Info.Id);
                TestHelper.CheckComponent(component.Info, child);
            }
        }
    }
}
