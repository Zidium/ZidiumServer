using Zidium.Api;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.Events
{
    public class GetEventsTests : BaseTest
    {
        /// <summary>
        /// Тест проверяет, что пустой фильтр должен искать любые события
        /// </summary>
        [Fact]
        public void EmptyFilterTest()
        {
            var component = TestHelper.CreateRandomComponent();
            var filter = new GetEventsData()
            {
                Category = EventCategory.ComponentEvent,
                OwnerId = component.Info.Id
            };
            
            // у нового компонента нет событий
            var client = component.Client;
            var events = client.ApiService.GetEvents(filter);
            Assert.Equal(events.Data.Count, 0);

            // отправим событие
            var eventMessage = TestHelper.CreateRandomComponentEvent(component);
            var sendEventResponse = eventMessage.Send();
            Assert.True(sendEventResponse.Success);

            // теперь фильтр должен найти 1 событие
            events = client.ApiService.GetEvents(filter);
            Assert.Equal(events.Data.Count, 1);
        }
    }
}
