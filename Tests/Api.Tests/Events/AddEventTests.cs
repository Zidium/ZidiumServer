using System;
using Xunit;
using Zidium.Api.Dto;
using Zidium.TestTools;

namespace Zidium.Api.Tests.Events
{
    public class AddEventTests : BaseTest
    {
        /// <summary>
        /// Тест проверет:
        /// 1. что события отправленные через офлайн-компоненты будут отправлены как появится интрнет
        /// 2. события склеиваются
        /// 3. при склейки событий меняется message
        /// </summary>
        [Fact]
        public void BasicTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();

            // запустим таймер с новым периодом
            client.Config.Events.EventManager.SendPeriod = TimeSpan.FromMinutes(30);
            client.Config.Access.WaitOnError = TimeSpan.Zero;
            
            // вычислим время
            Assert.True((client as Client).CanConvertToServerDate());

            var offlineWebService = new FakeApiService();
            var onlineWebService = client.ApiService;

            // выключим интернет
            client.SetApiService(offlineWebService);
            var component = account.CreateRandomComponentControl();

            // добавим событие в очередь
            string eventType = "test event " + Guid.NewGuid();
            string messageText = "test message " + Guid.NewGuid();
            var result = component.AddComponentEvent(eventType, messageText);
            Assert.Equal(AddEventStatus.WaitForSend, result.Status);

            component.Client.EventManager.Flush();
            Assert.Equal(AddEventStatus.WaitForSend, result.Status);
            Assert.True(component.IsFake());
            Assert.Equal(0, result.Errors); // потому что компонент фейковый

            // включим интернет
            client.SetApiService(onlineWebService);
            Assert.False(component.IsFake());
            component.Client.EventManager.Flush();
            Assert.Equal(AddEventStatus.Sended, result.Status);
            Assert.Equal(0, result.Errors);
            Assert.True(result.EventId.HasValue);

            // проверим, что событие записалось в БД
            var eventInfo = client.ApiService.GetEventById(result.EventId.Value);
            Assert.NotNull(eventInfo);
            Assert.Equal(eventType, eventInfo.GetDataAndCheck().TypeDisplayName);
            Assert.Equal(messageText, eventInfo.GetDataAndCheck().Message);
            Assert.Equal(1, eventInfo.GetDataAndCheck().Count);

            // продлим
            result = component.AddComponentEvent(eventType, messageText);
            Assert.Equal(AddEventStatus.WaitForJoin, result.Status);
            component.Client.EventManager.Flush();
            Assert.Equal(AddEventStatus.Joined, result.Status);

            // проверим, что продлилось
            account.SaveAllCaches();
            eventInfo = client.ApiService.GetEventById(result.EventId.Value);
            Assert.NotNull(eventInfo);
            Assert.Equal(eventType, eventInfo.GetDataAndCheck().TypeDisplayName);
            Assert.Equal(messageText, eventInfo.GetDataAndCheck().Message);
            Assert.Equal(2, eventInfo.GetDataAndCheck().Count);

            // продлим с новым message
            // события с разным message теперь не склеиваются
            /*
            string messageText2 = Guid.NewGuid().ToString();
            result = component.AddComponentEvent(eventType, messageText2);
            component.Client.EventManager.Flush();

            // проверим, что продлилось + изменился message
            AllCaches.SaveChanges();
            eventInfo = client.ApiService.GetEventById(result.EventId.Value);
            Assert.Equal(messageText2, eventInfo.Data.Message);
            Assert.Equal(3, eventInfo.Data.Count);
            */
        }

        /// <summary>
        /// Тест проверяет, что события можно клеить в циклах + это не повлияет на производительность и на трафик
        /// В тесте ниже за 5 сек производится склейка события много тысяч раз.
        /// </summary>
        [Fact]
        public void JoinTest()
        {
            var account = TestHelper.GetTestAccount();
            account.Config.Events.EventManager.SendPeriod = TimeSpan.FromMilliseconds(100);
            account.Config.Access.WaitOnError = TimeSpan.Zero;
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            
            // добавим событие в очередь в цикле много раз
            string eventType = "test event " + Guid.NewGuid();
            string messageText = "test message " + Guid.NewGuid();
            var startTime = DateTime.Now;
            var endTime = startTime.AddSeconds(10);
            while (DateTime.Now < endTime)
            {
                component.AddComponentEvent(eventType, messageText);
            }

            var result = component.AddComponentEvent(eventType, messageText);
            component.Client.EventManager.Flush();
            Assert.NotNull(result.EventId);

            // проверим, что событие склеилось много раз
            var eventInfo = client.ApiService.GetEventById(result.EventId.Value);
            Assert.True(eventInfo.GetDataAndCheck().Count > 10 * 000);
        }

        /// <summary>
        /// Тест проверяет, что метод AddApplicationError не теряет данных
        /// </summary>
        [Fact]
        public void AddApplicationErrorTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            
            // добавим событие в очередь
            string errorType = "test error " + Guid.NewGuid();
          
            var addResult = component
                .CreateApplicationError(errorType, "Сообщение")
                .Add();

            client.EventManager.Flush();
            Assert.NotNull(addResult.EventId);
            Guid eventId = addResult.EventId.Value;

            // проверим, что в БД
            var eventResponse = client.ApiService.GetEventById(eventId);
            Assert.True(eventResponse.Success, eventResponse.ErrorMessage);
            Assert.NotNull(eventResponse.GetDataAndCheck());
            Assert.Equal(errorType, eventResponse.GetDataAndCheck().TypeSystemName);
            Assert.Equal(errorType, eventResponse.GetDataAndCheck().TypeDisplayName);
            Assert.Equal("Сообщение", eventResponse.GetDataAndCheck().Message);
            Assert.Equal(EventCategory.ApplicationError, eventResponse.GetDataAndCheck().Category);
            Assert.Equal(EventImportance.Alarm, eventResponse.GetDataAndCheck().Importance);
            Assert.Equal(1, eventResponse.GetDataAndCheck().Count);

            // продлим 
            var addResult2 = component
                .CreateApplicationError(errorType, "Сообщение")
                .Add();

            client.EventManager.Flush();

            Assert.NotNull(addResult2.EventId);
            Assert.Equal(eventId, addResult2.EventId.Value);
            Assert.Equal(0, addResult2.Count); // счетчик сбрасывается после отправки

            account.SaveAllCaches();

            // проверим, что в БД
            eventResponse = client.ApiService.GetEventById(eventId);
            Assert.True(eventResponse.Success, eventResponse.ErrorMessage);
            Assert.NotNull(eventResponse.GetDataAndCheck());
            Assert.Equal(errorType, eventResponse.GetDataAndCheck().TypeSystemName);
            Assert.Equal(errorType, eventResponse.GetDataAndCheck().TypeDisplayName);
            Assert.Equal("Сообщение", eventResponse.GetDataAndCheck().Message);
            Assert.Equal(EventCategory.ApplicationError, eventResponse.GetDataAndCheck().Category);
            Assert.Equal(EventImportance.Alarm, eventResponse.GetDataAndCheck().Importance);
            Assert.Equal(2, eventResponse.GetDataAndCheck().Count);
        }

        /// <summary>
        /// Тест проверяет, что метод AddApplicationError не склеивает события, у которых Joinnterval = 0
        /// </summary>
        [Fact]
        public void AddApplicationErrorWithZeroJoinInterval1()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();

            // добавим событие в очередь 1-ый раз
            string errorType = "test erro " + Guid.NewGuid();
           
            var error = component
                .CreateApplicationError(errorType)
                .SetJoinInterval(TimeSpan.Zero)
                .SetStartDate(DateTime.Now);

            var addResult = error.Add();

            client.EventManager.Flush();
            Assert.True(addResult.EventId.HasValue);
            Assert.Equal(AddEventStatus.Sended, addResult.Status);
            Guid eventId1 = addResult.EventId.Value;

            // добавим событие в очередь 2-ый раз
            addResult = error.Add();
            client.EventManager.Flush();
            Assert.True(addResult.EventId.HasValue);
            Assert.Equal(AddEventStatus.Sended, addResult.Status);
            Guid eventId2 = addResult.EventId.Value;
            Assert.NotEqual(eventId1, eventId2);
        }

        /// <summary>
        /// Тест проверяет, что метод AddApplicationError не склеивает события, у которых Joinnterval = 0.
        /// При этом AddApplicationError вызывается 2 раза подряд до отправки
        /// </summary>
        [Fact]
        public void AddApplicationErrorWithZeroJoinInterval2()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();

            // добавим событие в очередь 1-ый раз
            string errorType = "test error " + Guid.NewGuid();

            var error = component
                .CreateApplicationError(errorType)
                .SetStartDate(DateTime.Now)
                .SetJoinInterval(TimeSpan.Zero);
            
            var addResult1 = error.Add();
            var addResult2 = error.Add();
            client.EventManager.Flush();
            
            Assert.True(addResult1.EventId.HasValue);
            Assert.True(addResult2.EventId.HasValue);
            Assert.Equal(AddEventStatus.Sended, addResult1.Status);
            Assert.Equal(AddEventStatus.Sended, addResult2.Status);
            Assert.NotEqual(addResult1.EventId.Value, addResult2.EventId.Value);
        }
    }
}
