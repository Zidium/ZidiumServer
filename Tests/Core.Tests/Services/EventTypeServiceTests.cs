﻿using System;
using Xunit;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Storage;

namespace Zidium.Core.Tests.Services
{
    public class EventTypeServiceTests : BaseTest
    {
        [Fact]
        public void GetOrCreateEventTypeUnicodeTest()
        {
            var storage = DependencyInjection.GetServicePersistent<IStorageFactory>().GetStorage();
            var timeService = DependencyInjection.GetServicePersistent<ITimeService>();
            var service = new EventTypeService(storage, timeService);
            var guid = Ulid.NewUlid();

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
