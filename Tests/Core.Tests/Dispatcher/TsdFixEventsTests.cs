using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;
using Zidium.Api;
using Zidium.Core.Api.Dispatcher;
using Zidium.Core.Common;
using Zidium.TestTools;
using EventCategory = Zidium.Core.Api.EventCategory;
using ThreadTaskQueue = Zidium.Api.ThreadTaskQueue;

namespace Zidium.Core.Tests.Dispatcher
{
    /// <summary>
    /// Баги ТСД событий
    /// </summary>
    public class TsdFixEventsTests
    {
        /// <summary>
        /// TSD отправляло каждый 10 минут событие диагностики с интервалом актуальности 20 минут
        /// Но колбаса событий компонента имела серые разрывы каждые 20 минут по 2 секунды длиной
        /// </summary>
        [Fact]
        public void ProlongComponentEventsStatus()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            Assert.False(component.IsFake());
            
            int count = 5;
            var period = TimeSpan.FromSeconds(5);
            for (int i = 0; i < count; i++)
            {
                var errorEvent = component
                    .CreateComponentEvent("Диагностика ТСД")
                    .SetImportance(EventImportance.Success)
                    .SetJoinInterval(TimeSpan.Zero);

                var response = errorEvent.Send();

                response.Check();
                Thread.Sleep(period);
            }

            using (var accountDbContext = account.CreateAccountDbContext())
            {
                // у колбасы событий должно быть только 2 статуса
                var events = accountDbContext
                    .Events
                    .Where(x => 
                        x.OwnerId == component.Info.Id 
                        && x.Category == EventCategory.ComponentEventsStatus)
                    .OrderBy(x => x.CreateDate)
                    .ToList();

                Assert.Equal(2, events.Count);

                // у колбасы итогового статуса тоже только 2 статуса
                events = accountDbContext
                    .Events
                    .Where(x =>
                        x.OwnerId == component.Info.Id
                        && x.Category == EventCategory.ComponentExternalStatus)
                    .OrderBy(x => x.CreateDate)
                    .ToList();

                Assert.Equal(2, events.Count);
            }
        }

        /// <summary>
        /// Метод диспетчера SendEvent должен создавать только 1 событие 
        /// </summary>
        [Fact]
        public void ProcessSimpleEventThreadSafe2()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            const int threadCount = 5;
            var queue = new ThreadTaskQueue(threadCount);
            var responses = new List<SendEventResponse>();
            var action = new ThreadStart(() =>
            {
                var response = component
                    .CreateApplicationError("test")
                    .SetJoinInterval(TimeSpan.FromHours(1))
                    .Send();

                lock (responses)
                {
                    responses.Add(response);
                }
            });
            for (int i = 0; i < threadCount; i++)
            {
                queue.Add(action);
            }
            queue.WaitForAllTasksCompleted();

            // все запросы выполнены успешно
            var successCount = responses.Count(x => x.Success);
            Assert.Equal(threadCount, successCount);

            // получилось 1 событие
            var eventIds = responses.Select(x => x.Data.EventId).Distinct().ToList();
            Assert.Equal(1, eventIds.Count);
        }

        private void SendTsdEvent(object componentObj)
        {
            var component = componentObj as IComponentControl;

            var response = component
                .CreateComponentEvent("test")
                .SetImportance(EventImportance.Success)
                .SetJoinInterval(TimeSpan.FromSeconds(4))
                .Send();

            if (response.Success == false)
            {
                response.Check();
            }
        }

        private void UpdateTsdStatus(object componentObj)
        {
            Thread.Sleep(1000 + RandomHelper.GetRandomInt32(0, 7000));
            var component = componentObj as IComponentControl;
            var accountId = tsdAccountId;
            var componentId = component.Info.Id;
            var dispatcherClient = new DispatcherClient("Test");
            var response = dispatcherClient.UpdateComponentState(accountId, componentId);
            if (response.Success == false)
            {
                response.Check();
            }
        }

        private Guid tsdAccountId;

        /// <summary>
        /// Была ошибка в UpdateLastStatusId связанная с тем, при одновременной обработке события
        /// Создавалис дубли в eventObj.StatusEvents.Add(statusEvent);
        /// Т.к. не выполняось условие 
        /// if (eventObj.Id == Guid.Empty || eventObj.LastStatusEventId == statusEvent.Id)
        /// {
        ///     return;
        /// }
        /// из-за того, что второй поток не знал актуальный eventObj.LastStatusEventId 
        /// </summary>
        [Fact]
        public void TsdBug()
        {
            var account = TestHelper.GetTestAccount();
            tsdAccountId = account.Id;
            var component = account.CreateRandomComponentControl();
            var timer1 = new Timer(SendTsdEvent, component, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            var timer2 = new Timer(UpdateTsdStatus, component, TimeSpan.Zero, TimeSpan.FromMilliseconds(500));
            Thread.Sleep(30 * 1000);
            timer1.Dispose();
            timer2.Dispose();
        }
    }
}
