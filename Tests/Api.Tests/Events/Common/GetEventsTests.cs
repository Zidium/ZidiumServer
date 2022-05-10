using Xunit;
using Zidium.Api.Dto;
using Zidium.TestTools;

namespace Zidium.Api.Tests.Events
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
            var filter = new GetEventsRequestDataDto()
            {
                Category = EventCategory.ComponentEvent,
                OwnerId = component.Info.Id
            };
            
            // у нового компонента нет событий
            var client = component.Client;
            var events = client.ApiService.GetEvents(filter);
            Assert.Empty(events.GetDataAndCheck());

            // отправим событие
            var eventMessage = TestHelper.CreateRandomComponentEvent(component);
            var sendEventResponse = eventMessage.Send();
            sendEventResponse.Check();

            // теперь фильтр должен найти 1 событие
            events = client.ApiService.GetEvents(filter);
            Assert.Single(events.GetDataAndCheck());
        }
    }
}
