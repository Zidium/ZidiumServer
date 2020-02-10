using System;
using System.Web.Mvc;
using Zidium.Core.Api;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class TimelineController : ContextController
    {
        // Рисует диаграмму указанных статусов для компонента
        public ActionResult ForComponent(Guid id, EventCategory category, DateTime? fromDate, DateTime? toDate)
        {
            return ForOwner(id, category, fromDate, toDate);
        }

        // Рисует диаграмму указанных статусов для проверки
        public ActionResult ForUnitTest(Guid id, EventCategory category, DateTime? fromDate, DateTime? toDate)
        {
            var unitTestRepository = CurrentAccountDbContext.GetUnitTestRepository();
            var unitTest = unitTestRepository.GetById(id);
            return ForOwner(unitTest.Id, category, fromDate, toDate);
        }

        public ActionResult ForUnitTestByInterval(Guid id, EventCategory category, TimelineInterval interval)
        {
            var toDate = MvcApplication.GetServerDateTime();
            var fromDate = TimelineHelper.IntervalToStartDate(toDate, interval);
            return ForUnitTest(id, category, fromDate, toDate);
        }

        // Рисует диаграмму указанных статусов для метрики
        public ActionResult ForMetric(Guid id, EventCategory category, DateTime? fromDate, DateTime? toDate)
        {
            var metricRepository = CurrentAccountDbContext.GetMetricRepository();
            var metric = metricRepository.GetById(id);
            return ForOwner(metric.Id, category, fromDate, toDate);
        }

        public ActionResult ForMetricByInterval(Guid id, EventCategory category, TimelineInterval interval)
        {
            var toDate = MvcApplication.GetServerDateTime();
            var fromDate = TimelineHelper.IntervalToStartDate(toDate, interval);
            return ForMetric(id, category, fromDate, toDate);
        }

        // Рисует диаграмму указанных статусов для владельца
        protected ActionResult ForOwner(Guid ownerId, EventCategory category, DateTime? fromDate, DateTime? toDate)
        {
            var eDate = toDate ?? DateTime.Now;
            var sDate = fromDate ?? eDate.AddHours(-24);

            var eventRepository = CurrentAccountDbContext.GetEventRepository();

            var states = eventRepository.GetTimelineStates(ownerId, category, sDate, eDate);
            var items = this.GetTimelineItemsByStates(states, sDate, eDate);
            var okTime = eventRepository.GetTimelineOkTime(states, sDate, eDate);

            var model = new TimelineModel()
            {
                Category = category,
                OwnerId = ownerId,
                DateFrom = sDate,
                DateTo = eDate,
                Items = items,
                OkTime = okTime
            };

            return PartialView("TimelinePartial", model);
        }

        // Рисует диаграмму указанных статусов для типа события по одному компоненту
        public ActionResult ForEventType(Guid id, Guid eventTypeId, DateTime? fromDate, DateTime? toDate, bool? hideUptime)
        {
            var eDate = toDate ?? DateTime.Now;
            var sDate = fromDate ?? eDate.AddHours(-24);

            var eventType = GetEventTypeById(eventTypeId);
            var eventRepository = CurrentAccountDbContext.GetEventRepository();

            var states = eventRepository.GetTimelineStates(id, eventTypeId, sDate, eDate);
            var items = this.GetTimelineItemsByStates(states, sDate, eDate);
            var okTime = eventRepository.GetTimelineOkTime(states, sDate, eDate);

            var model = new TimelineModel()
            {
                Category = eventType.Category,
                OwnerId = id,
                DateFrom = sDate,
                DateTo = eDate,
                Items = items,
                OkTime = okTime,
                EventTypeId = eventTypeId,
                HideUptime = hideUptime ?? false
            };

            return PartialView("TimelinePartial", model);
        }

        // Рисует диаграмму указанных статусов для типа события независимо от компонента
        public ActionResult ForEventTypeAnyComponents(Guid eventTypeId, DateTime? fromDate, DateTime? toDate, bool? hideUptime)
        {
            var eDate = toDate ?? DateTime.Now;
            var sDate = fromDate ?? eDate.AddHours(-24);

            var eventRepository = CurrentAccountDbContext.GetEventRepository();

            var states = eventRepository.GetTimelineStatesAnyComponents(eventTypeId, sDate, eDate);
            var items = this.GetTimelineItemsByStates(states, sDate, eDate);
            var okTime = eventRepository.GetTimelineOkTime(states, sDate, eDate);

            var model = new TimelineModel()
            {
                Category = null,
                OwnerId = null,
                DateFrom = sDate,
                DateTo = eDate,
                Items = items,
                OkTime = okTime,
                EventTypeId = eventTypeId,
                HideUptime = hideUptime ?? false
            };

            return PartialView("TimelinePartial", model);
        }

        public ActionResult ForEventTypeAnyComponentsByInterval(Guid eventTypeId, TimelineInterval interval, bool? hideUptime)
        {
            var toDate = MvcApplication.GetServerDateTime();
            var fromDate = TimelineHelper.IntervalToStartDate(toDate, interval);
            return ForEventTypeAnyComponents(eventTypeId, fromDate, toDate, hideUptime);
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        public TimelineController(Guid accountId, Guid userId) : base(accountId, userId) { }

        public TimelineController() { }

    }
}