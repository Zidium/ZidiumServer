using System;
using Xunit;
using Zidium.Api.Dto;
using Zidium.Core.AccountsDb;
using Zidium.Storage;

namespace Zidium.Core.Tests.Services
{
    public class EventTypeServiceTests : BaseTest
    {
        [Fact]
        public void GetOrCreateEventTypeUnicodeTest()
        {
            var storage = DependencyInjection.GetServicePersistent<IStorageFactory>().GetStorage();
            var service = new EventTypeService(storage);
            var guid = Guid.NewGuid();

            EventTypeForAdd eventType1ForAdd = new EventTypeForAdd()
            {
                Category = EventCategory.ComponentEvent,
                SystemName = "Русское название " + guid,
                DisplayName = "Русское название " + guid
            };

            EventTypeForAdd eventType2ForAdd = new EventTypeForAdd()
            {
                Category = EventCategory.ComponentEvent,
                SystemName = "русское Название " + guid,
                DisplayName = "русское Название " + guid
            };

            var eventType1 = service.GetOrCreate(eventType1ForAdd);
            var eventType2 = service.GetOrCreate(eventType2ForAdd);

            Assert.Equal(eventType1.Id, eventType2.Id);
        }
    }
}
