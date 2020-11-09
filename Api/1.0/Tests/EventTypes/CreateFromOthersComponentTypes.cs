using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.EventTypes
{
    public class CreateFromOthersComponentTypes : BaseTest
    {
        /// <summary>
        /// Тест проверяет, что 2 события с одинаковым системным именем будут иметь одинаковые ИД типов, 
        /// если они отправлены от компонентов разных типов.
        /// Раньше логика была другая, разные типы с одинаковым именем имели разные ИД
        /// </summary>
        [Fact]
        public void Test() // todo нужно подумать нужно ли так делать, может быть так нужно сделать только для ошибок?
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var root = client.GetRootComponentControl();
            var componentType1 = client.GetOrCreateComponentTypeControl("componentType1");
            var componentType2 = client.GetOrCreateComponentTypeControl("componentType2");
            var component1 = root.GetOrCreateChildComponentControl(componentType1, "component1");
            var component2 = root.GetOrCreateChildComponentControl(componentType2, "component2");
            var eventType = "MyEventType " + TestHelper.GetRandomSystemName();
            var response1 = component1.CreateComponentEvent(eventType).Send();
            var response2 = component2.CreateComponentEvent(eventType).Send();
            response1.Check();
            response2.Check();
            Assert.Equal(response1.Data.EventTypeId, response2.Data.EventTypeId);
        }
    }
}
