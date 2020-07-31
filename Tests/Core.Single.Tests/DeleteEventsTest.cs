using System;
using System.Threading;
using NLog;
using Xunit;
using Zidium.Agent.AgentTasks.DeleteEvents;
using Zidium.Core.Api;
using Zidium.Storage;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Core.Single.Tests
{
    public class DeleteEventsTest
    {
        [Fact]
        public void DeleteCustomerEventsTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var eventType = TestHelper.GetTestEventType(account.Id);

            // Установим время хранения событий 30 дней
            var dispatcher = DispatcherHelper.GetDispatcherService();

            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data.Hard;

            limits.EventsMaxDays = 30;

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits,
                    Type = TariffLimitType.Hard
                }
            }).Check();

            // Выполним предварительную очистку событий
            var processor = new DeleteCustomerEventsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.Process(account.Id);
            Assert.Null(processor.DbProcessor.FirstException);

            // Добавим одно старое и одно новое событие каждой категории

            var oldDate = DateTime.Now.AddDays(-31);

            AddEvent(account.Id, component.Info.Id, oldDate, EventCategory.ComponentEvent, eventType.Id);
            AddEvent(account.Id, component.Info.Id, DateTime.Now, EventCategory.ComponentEvent, eventType.Id);

            AddEvent(account.Id, component.Info.Id, oldDate, EventCategory.ApplicationError, eventType.Id);
            AddEvent(account.Id, component.Info.Id, DateTime.Now, EventCategory.ApplicationError, eventType.Id);

            AddEvent(account.Id, component.Info.Id, oldDate, EventCategory.ComponentEventsStatus, eventType.Id);
            AddEvent(account.Id, component.Info.Id, DateTime.Now, EventCategory.ComponentEventsStatus, eventType.Id);

            account.SaveAllCaches();

            // Удалим старые события
            processor.Process(account.Id);

            // Проверим, что нет ошибок и удалены 3 события

            Assert.Null(processor.DbProcessor.FirstException);
            Assert.Equal(3, processor.DeletedEventsCount);
            Assert.Equal(3, processor.DeletedPropertiesCount);
        }

        [Fact]
        public void DeleteUnittestEventsTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var unittest = TestHelper.CreateTestUnitTest(account.Id, component.Info.Id);
            var eventType = TestHelper.GetTestEventType(account.Id);

            // Установим время хранения проверок 30 дней
            var dispatcher = DispatcherHelper.GetDispatcherService();

            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data.Hard;

            limits.UnitTestsMaxDays = 30;

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits,
                    Type = TariffLimitType.Hard
                }
            }).Check();

            // Выполним предварительную очистку событий
            var processor = new DeleteUnittestEventsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.Process(account.Id);
            Assert.Null(processor.DbProcessor.FirstException);

            // Добавим одно старое и одно новое событие каждой категории

            var oldDate = DateTime.Now.AddDays(-31);

            AddEvent(account.Id, unittest.Id, oldDate, EventCategory.UnitTestStatus, eventType.Id);
            AddEvent(account.Id, unittest.Id, DateTime.Now, EventCategory.UnitTestStatus, eventType.Id);

            AddEvent(account.Id, unittest.Id, oldDate, EventCategory.UnitTestResult, eventType.Id);
            AddEvent(account.Id, unittest.Id, DateTime.Now, EventCategory.UnitTestResult, eventType.Id);

            AddEvent(account.Id, component.Info.Id, oldDate, EventCategory.ComponentUnitTestsStatus, eventType.Id);
            AddEvent(account.Id, component.Info.Id, DateTime.Now, EventCategory.ComponentUnitTestsStatus, eventType.Id);

            account.SaveAllCaches();

            // Удалим старые события
            processor.Process(account.Id);

            // Проверим, что нет ошибок и удалены 3 события

            Assert.Null(processor.DbProcessor.FirstException);
            Assert.Equal(3, processor.DeletedEventsCount);
            Assert.Equal(3, processor.DeletedPropertiesCount);
        }

        [Fact]
        public void DeleteMetricEventsTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var metric = TestHelper.CreateTestMetric(account.Id, component.Info.Id);
            var eventType = TestHelper.GetTestEventType(account.Id);

            // Установим время хранения метрик 30 дней
            var dispatcher = DispatcherHelper.GetDispatcherService();

            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data.Hard;

            limits.MetricsMaxDays = 30;

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits,
                    Type = TariffLimitType.Hard
                }
            }).Check();

            // Выполним предварительную очистку событий
            var processor = new DeleteMetricEventsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.Process(account.Id);
            Assert.Null(processor.DbProcessor.FirstException);

            // Добавим одно старое и одно новое событие каждой категории

            var oldDate = DateTime.Now.AddDays(-31);

            AddEvent(account.Id, metric.Id, oldDate, EventCategory.MetricStatus, eventType.Id);
            AddEvent(account.Id, metric.Id, DateTime.Now, EventCategory.MetricStatus, eventType.Id);

            AddEvent(account.Id, component.Info.Id, oldDate, EventCategory.ComponentMetricsStatus, eventType.Id);
            AddEvent(account.Id, component.Info.Id, DateTime.Now, EventCategory.ComponentMetricsStatus, eventType.Id);

            account.SaveAllCaches();

            // Удалим старые события
            processor.Process(account.Id);

            // Проверим, что нет ошибок и удалены 2 события

            Assert.Null(processor.DbProcessor.FirstException);
            Assert.Equal(2, processor.DeletedEventsCount);
            Assert.Equal(2, processor.DeletedPropertiesCount);
        }

        [Fact]
        public void DeleteComponentEventsTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var eventType = TestHelper.GetTestEventType(account.Id);

            // Установим время хранения всех категорий событий 30 дней
            var dispatcher = DispatcherHelper.GetDispatcherService();

            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data.Hard;

            limits.EventsMaxDays = 30;
            limits.UnitTestsMaxDays = 30;
            limits.MetricsMaxDays = 30;

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits,
                    Type = TariffLimitType.Hard
                }
            }).Check();

            // Выполним предварительную очистку событий
            var processor = new DeleteComponentEventsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.Process(account.Id);
            Assert.Null(processor.DbProcessor.FirstException);

            // Добавим одно старое и одно новое событие каждой категории

            var oldDate = DateTime.Now.AddDays(-31);

            AddEvent(account.Id, component.Info.Id, oldDate, EventCategory.ComponentExternalStatus, eventType.Id);
            AddEvent(account.Id, component.Info.Id, DateTime.Now, EventCategory.ComponentExternalStatus, eventType.Id);

            AddEvent(account.Id, component.Info.Id, oldDate, EventCategory.ComponentInternalStatus, eventType.Id);
            AddEvent(account.Id, component.Info.Id, DateTime.Now, EventCategory.ComponentInternalStatus, eventType.Id);

            AddEvent(account.Id, component.Info.Id, oldDate, EventCategory.ComponentChildsStatus, eventType.Id);
            AddEvent(account.Id, component.Info.Id, DateTime.Now, EventCategory.ComponentChildsStatus, eventType.Id);

            account.SaveAllCaches();

            // Удалим старые события
            processor.Process(account.Id);

            // Проверим, что нет ошибок и удалены 3 события

            Assert.Null(processor.DbProcessor.FirstException);
            Assert.Equal(3, processor.DeletedEventsCount);
            Assert.Equal(3, processor.DeletedPropertiesCount);
        }

        protected void AddEvent(Guid accountId, Guid ownerId, DateTime date, EventCategory category, Guid eventTypeId)
        {
            using (var storageDbContext = TestHelper.GetAccountDbContext(accountId))
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
