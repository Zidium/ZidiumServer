using System;
using System.Threading;
using NLog;
using Xunit;
using Zidium.Agent.AgentTasks.DeleteEvents;
using Zidium.Api.Dto;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Agent.Single.Tests
{
    public class DeleteEventsTest : BaseTest
    {
        [Fact]
        public void DeleteCustomerEventsTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var eventType = TestHelper.GetTestEventType();

            // Выполним предварительную очистку событий
            var processor = new DeleteCustomerEventsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.Process();

            // Добавим одно старое и одно новое событие каждой категории

            var oldDate = DateTime.Now.AddDays(-31);

            AddEvent(component.Info.Id, oldDate, EventCategory.ComponentEvent, eventType.Id);
            AddEvent(component.Info.Id, DateTime.Now, EventCategory.ComponentEvent, eventType.Id);

            AddEvent(component.Info.Id, oldDate, EventCategory.ApplicationError, eventType.Id);
            AddEvent(component.Info.Id, DateTime.Now, EventCategory.ApplicationError, eventType.Id);

            AddEvent(component.Info.Id, oldDate, EventCategory.ComponentEventsStatus, eventType.Id);
            AddEvent(component.Info.Id, DateTime.Now, EventCategory.ComponentEventsStatus, eventType.Id);

            account.SaveAllCaches();

            // Удалим старые события
            processor.Process();

            // Проверим, что удалены 3 события
            Assert.Equal(3, processor.DeletedEventsCount);
            Assert.Equal(3, processor.DeletedPropertiesCount);
        }

        [Fact]
        public void DeleteUnittestEventsTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var unittest = TestHelper.CreateTestUnitTest(component.Info.Id);
            var eventType = TestHelper.GetTestEventType();

            // Выполним предварительную очистку событий
            var processor = new DeleteUnittestEventsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.Process();

            // Добавим одно старое и одно новое событие каждой категории

            var oldDate = DateTime.Now.AddDays(-31);

            AddEvent(unittest.Id, oldDate, EventCategory.UnitTestStatus, eventType.Id);
            AddEvent(unittest.Id, DateTime.Now, EventCategory.UnitTestStatus, eventType.Id);

            AddEvent(unittest.Id, oldDate, EventCategory.UnitTestResult, eventType.Id);
            AddEvent(unittest.Id, DateTime.Now, EventCategory.UnitTestResult, eventType.Id);

            AddEvent(component.Info.Id, oldDate, EventCategory.ComponentUnitTestsStatus, eventType.Id);
            AddEvent(component.Info.Id, DateTime.Now, EventCategory.ComponentUnitTestsStatus, eventType.Id);

            account.SaveAllCaches();

            // Удалим старые события
            processor.Process();

            // Проверим, что удалены 3 события
            Assert.Equal(3, processor.DeletedEventsCount);
            Assert.Equal(3, processor.DeletedPropertiesCount);
        }

        [Fact]
        public void DeleteMetricEventsTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var metric = TestHelper.CreateTestMetric(component.Info.Id);
            var eventType = TestHelper.GetTestEventType();

            // Выполним предварительную очистку событий
            var processor = new DeleteMetricEventsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.Process();

            // Добавим одно старое и одно новое событие каждой категории

            var oldDate = DateTime.Now.AddDays(-31);

            AddEvent(metric.Id, oldDate, EventCategory.MetricStatus, eventType.Id);
            AddEvent(metric.Id, DateTime.Now, EventCategory.MetricStatus, eventType.Id);

            AddEvent(component.Info.Id, oldDate, EventCategory.ComponentMetricsStatus, eventType.Id);
            AddEvent(component.Info.Id, DateTime.Now, EventCategory.ComponentMetricsStatus, eventType.Id);

            account.SaveAllCaches();

            // Удалим старые события
            processor.Process();

            // Проверим, что удалены 2 события
            Assert.Equal(2, processor.DeletedEventsCount);
            Assert.Equal(2, processor.DeletedPropertiesCount);
        }

        [Fact]
        public void DeleteComponentEventsTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var eventType = TestHelper.GetTestEventType();

            // Выполним предварительную очистку событий
            var processor = new DeleteComponentEventsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.Process();

            // Добавим одно старое и одно новое событие каждой категории

            var oldDate = DateTime.Now.AddDays(-31);

            AddEvent(component.Info.Id, oldDate, EventCategory.ComponentExternalStatus, eventType.Id);
            AddEvent(component.Info.Id, DateTime.Now, EventCategory.ComponentExternalStatus, eventType.Id);

            AddEvent(component.Info.Id, oldDate, EventCategory.ComponentInternalStatus, eventType.Id);
            AddEvent(component.Info.Id, DateTime.Now, EventCategory.ComponentInternalStatus, eventType.Id);

            AddEvent(component.Info.Id, oldDate, EventCategory.ComponentChildsStatus, eventType.Id);
            AddEvent(component.Info.Id, DateTime.Now, EventCategory.ComponentChildsStatus, eventType.Id);

            account.SaveAllCaches();

            // Удалим старые события
            processor.Process();

            // Проверим, что удалены 3 события
            Assert.Equal(3, processor.DeletedEventsCount);
            Assert.Equal(3, processor.DeletedPropertiesCount);
        }

        protected void AddEvent(Guid ownerId, DateTime date, EventCategory category, Guid eventTypeId)
        {
            using (var storageDbContext = TestHelper.GetDbContext())
            {
                var _event = new DbEvent()
                {
                    Id = Guid.NewGuid(),
                    OwnerId = ownerId,
                    Category = category,
                    StartDate = date,
                    CreateDate = date,
                    EndDate = date,
                    ActualDate = date,
                    LastUpdateDate = date,
                    EventTypeId = eventTypeId
                };

                _event.Properties.Add(new DbEventProperty()
                {
                    Id = Guid.NewGuid(),
                    EventId = _event.Id,
                    Event = _event,
                    DataType = DataType.String,
                    Name = "test name",
                    Value = "test value"
                });

                storageDbContext.Events.Add(_event);

                storageDbContext.SaveChanges();
            }
        }
    }
}
