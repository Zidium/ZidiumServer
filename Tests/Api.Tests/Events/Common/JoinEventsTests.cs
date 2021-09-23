using System;
using System.Linq;
using Xunit;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.TestTools;

namespace Zidium.Api.Tests.Events
{
    public class JoinEventsTests : BaseTest
    {
        /// <summary>
        /// Проверим, что 2-ое событие с 0-ым интервалом склейки не приклеится.
        /// StartDate у событий одинаковые.
        /// </summary>
        [Fact]
        public void ZeroJoinInterval()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            // отправляем сообщение 1 раз
            DateTime start = TestHelper.GetNow();
            var error = TestHelper.CreateRandomApplicationError(component);
            error.JoinInterval = TimeSpan.FromMinutes(10);
            error.StartDate = start;
            var response1 = error.Send();
            Assert.True(response1.Success);

            // отправляем сообщение 2 раз
            error.JoinInterval = TimeSpan.Zero;
            var response2 = error.Send();
            Assert.True(response2.Success);
            Assert.NotEqual(response1.GetDataAndCheck().EventId, response2.GetDataAndCheck().EventId);
        }

        /// <summary>
        /// Проверим, что события умеют склеиваться
        /// </summary>
        [Fact]
        public void NormalJoin()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            // Создадим событие
            DateTime start = TestHelper.GetNow();
            var error = TestHelper.CreateRandomApplicationError(component);
            error.JoinInterval = TimeSpan.FromMinutes(10);
            error.StartDate = start;

            // отправляем сообщение 1 раз
            var response1 = error.Send();
            Assert.True(response1.Success);

            // отправляем сообщение 2 раз
            var response2 = error.Send();
            Assert.True(response2.Success);
            Assert.Equal(response1.GetDataAndCheck().EventId, response2.GetDataAndCheck().EventId);
        }

        /// <summary>
        /// Тест проверяет, что события с одинаковыми версиями склеиваются, 
        /// а с разными версиями НЕ склеиваются.
        /// </summary>
        [Fact]
        public void DiffVersions()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();

            // version = null
            var component = client.GetRootComponentControl().GetOrCreateChildComponentControl("testType", "testName");
            Assert.Null(component.Info.Version);
            var error = TestHelper.CreateRandomApplicationError(component);
            error.Version = null;
            error.JoinInterval = TimeSpan.FromMinutes(10);
            var response1 = error.Send();
            var response2 = error.Send();
            Assert.Null(error.Version);
            Assert.Equal(response1.GetDataAndCheck().EventId, response2.GetDataAndCheck().EventId);

            // version = 1.0
            error.Version = "1.0";
            var response3 = error.Send();
            var response4 = error.Send();
            Assert.NotEqual(response3.GetDataAndCheck().EventId, response2.GetDataAndCheck().EventId); 
            Assert.Equal(response4.GetDataAndCheck().EventId, response3.GetDataAndCheck().EventId);

            // version = 1.1
            error.Version = "1.1";
            var response5 = error.Send();
            var response6 = error.Send();
            Assert.NotEqual(response5.GetDataAndCheck().EventId, response4.GetDataAndCheck().EventId);
            Assert.Equal(response5.GetDataAndCheck().EventId, response6.GetDataAndCheck().EventId);
        }

        [Fact]
        public void DiffImportanceSend()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var error = TestHelper.CreateRandomApplicationError(component);

            // alarm
            error.Importance = EventImportance.Alarm;
            error.JoinInterval = TimeSpan.FromMinutes(10);
            var response1 = error.Send();
            var response2 = error.Send();
            Assert.Equal(response1.GetDataAndCheck().EventId, response2.GetDataAndCheck().EventId);

            // warning
            error.Importance = EventImportance.Warning;
            var response3 = error.Send();
            Assert.NotEqual(response3.GetDataAndCheck().EventId, response2.GetDataAndCheck().EventId);
        }

        [Fact]
        public void DiffImportanceAdd()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var errorType = "test error type " + Ulid.NewUlid();
            var joinInterval = TimeSpan.FromMinutes(10);

            // отправим 2 события
            const int count = 5;
            for (int i = 0; i < count; i++)
            {
                // alarm
                component.CreateApplicationError(errorType)
                    .SetImportance(EventImportance.Alarm)
                    .SetJoinInterval(joinInterval)
                    .Add();

                // warning
                component.CreateApplicationError(errorType)
                    .SetImportance(EventImportance.Warning)
                    .SetJoinInterval(joinInterval)
                    .Add();
            }
            component.Client.EventManager.Flush();

            // проверим, что события два
            var client = component.Client;
            var events = client.ApiService.GetEvents(new GetEventsRequestDataDto()
            {
                Category = EventCategory.ApplicationError,
                OwnerId = component.Info.Id
            }).GetDataAndCheck();
            Assert.Equal(2, events.Count);
            Assert.True(events.All(x => x.Count == count));
        }

        [Fact]
        public void DiffMessageTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var errorType = "test error type " + Ulid.NewUlid();
            var joinInterval = TimeSpan.FromMinutes(10);

            // Отправим два события с разными сообщениями

            component.CreateApplicationError(errorType, "Message1")
                .SetImportance(EventImportance.Alarm)
                .SetJoinInterval(joinInterval)
                .Add();

            component.CreateApplicationError(errorType, "Message2")
                .SetImportance(EventImportance.Alarm)
                .SetJoinInterval(joinInterval)
                .Add();

            component.Client.EventManager.Flush();

            // проверим, что события не склеились
            var client = component.Client;
            var events = client.ApiService.GetEvents(new GetEventsRequestDataDto()
            {
                Category = EventCategory.ApplicationError,
                OwnerId = component.Info.Id
            }).GetDataAndCheck();
            Assert.Equal(2, events.Count);
        }

        [Fact]
        public void DiffPropertyTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var errorType = "test error type " + Ulid.NewUlid();
            var joinInterval = TimeSpan.FromMinutes(10);

            // Отправим два события с разными доп. свойствами

            component.CreateApplicationError(errorType)
                .SetImportance(EventImportance.Alarm)
                .SetJoinInterval(joinInterval)
                .SetProperty("Prop", 1)
                .Add();

            component.CreateApplicationError(errorType)
                .SetImportance(EventImportance.Alarm)
                .SetJoinInterval(joinInterval)
                .SetProperty("Prop", 2)
                .Add();

            component.Client.EventManager.Flush();

            // проверим, что события не склеились
            var client = component.Client;
            var events = client.ApiService.GetEvents(new GetEventsRequestDataDto()
            {
                Category = EventCategory.ApplicationError,
                OwnerId = component.Info.Id
            }).GetDataAndCheck();
            Assert.Equal(2, events.Count);
        }

    }
}
