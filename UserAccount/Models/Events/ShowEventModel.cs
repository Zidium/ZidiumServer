using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Api.Dto;
using Zidium.Core;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.UserAccount.Models.Events;

namespace Zidium.UserAccount.Models
{
    public class ShowEventModel
    {
        public IStorage Storage { get; set; }

        public EventForRead Event { get; set; }

        public EventPropertyForRead[] Properties { get; set; }

        public List<ShowEventModelReason> ErrorReasons { get; set; }

        public List<ShowEventModelReason> UnitTestsReasons { get; set; }

        public List<ShowEventModelReason> MetricsReasons { get; set; }

        public List<ShowEventModelReason> ChildsReasons { get; set; }

        public bool HasReasons
        {
            get
            {
                return ErrorReasons.Count > 0
                       || UnitTestsReasons.Count > 0
                       || MetricsReasons.Count > 0
                       || ChildsReasons.Count > 0;
            }
        }

        public const int MaxEventsReasonCount = 100;

        public MonitoringStatus GetMonitoringStatus()
        {
            return MonitoringStatusHelper.Get(Event.Importance);
        }

        public string PageTitle { get; protected set; }

        public string GetEventMessageText(EventForRead eventObj, EventTypeForRead eventType)
        {
            if (string.IsNullOrEmpty(eventObj.Message))
            {
                return eventType.DisplayName;
            }
            return eventObj.Message;
        }

        public EventTypeForRead EventType { get; set; }

        public ComponentForRead Component { get; set; }

        public MetricForRead Metric { get; set; }

        public MetricTypeForRead MetricType { get; set; }

        public UnitTestForRead UnitTest { get; set; }

        protected string GetPageTitle(EventCategory category, EventTypeForRead eventType)
        {
            switch (category)
            {
                case EventCategory.ApplicationError:
                    if (eventType.Code != null)
                    {
                        return "Ошибка " + eventType.Code;
                    }
                    return "Ошибка";
                case EventCategory.ComponentEvent:
                    if (eventType.Code != null)
                    {
                        return "Событие компонента " + eventType.Code;
                    }
                    return "Событие компонента";
                case EventCategory.ComponentExternalStatus:
                    return "Итоговый статус компонента";
                case EventCategory.ComponentInternalStatus:
                    return "Внутренний статус компонента";
                case EventCategory.ComponentEventsStatus:
                    return "Статус всех событий компонента";
                case EventCategory.ComponentUnitTestsStatus:
                    return "Статус всех проверок компонента";
                case EventCategory.ComponentMetricsStatus:
                    return "Статус всех метрик компонента";
                case EventCategory.ComponentChildsStatus:
                    return "Статус всех детей компонента";
                case EventCategory.MetricStatus:
                    return "Статус метрики";
                case EventCategory.UnitTestResult:
                    return "Результат проверки";
                case EventCategory.UnitTestStatus:
                    return "Статус проверки";
                default:
                    throw new Exception("Неизвестная категория " + category);

            }
        }

        public void Init(Guid eventId, UserInfo currentUser, IStorage storage)
        {
            Storage = storage;

            ErrorReasons = new List<ShowEventModelReason>();
            UnitTestsReasons = new List<ShowEventModelReason>();
            MetricsReasons = new List<ShowEventModelReason>();
            ChildsReasons = new List<ShowEventModelReason>();

            Event = storage.Events.GetOneById(eventId);
            Properties = storage.EventProperties.GetByEventId(eventId);

            // тип события
            EventType = storage.EventTypes.GetOneById(Event.EventTypeId);

            // событие метрики
            if (Event.Category == EventCategory.MetricStatus)
            {
                var metric = storage.Metrics.GetOneById(Event.OwnerId);
                Metric = metric;
                MetricType = storage.MetricTypes.GetOneById(metric.MetricTypeId);
                Component = storage.Components.GetOneById(metric.ComponentId);
            }
            // событие юнит-теста
            else if (Event.Category == EventCategory.UnitTestStatus ||
                     Event.Category == EventCategory.UnitTestResult)
            {
                var unitTest = storage.UnitTests.GetOneById(Event.OwnerId);
                UnitTest = unitTest;
                Component = storage.Components.GetOneById(unitTest.ComponentId);
            }
            else // событие компонента
            {
                Component = storage.Components.GetOneById(Event.OwnerId);
            }

            // название страницы
            PageTitle = "Событие - " + GetPageTitle(Event.Category, EventType).ToLowerInvariant();

            // причины статусов
            if (Event.Category.IsStatus())
            {
                LoadReasons(Event, storage);
            }

            CanChangeImportance = !EventType.IsSystem;
            CanChangeActuality = Event.Category.IsComponentCategory() && !Event.Category.IsStatus() && (Event.ActualDate > DateTime.UtcNow);

            // Оставим только 100 последних причин, иначе они долго рассчитываются и выводятся
            UnitTestsReasons = UnitTestsReasons.OrderByDescending(t => t.Event.StartDate).Take(100).ToList();
            MetricsReasons = MetricsReasons.OrderByDescending(t => t.Event.StartDate).Take(100).ToList();
            ChildsReasons = ChildsReasons.OrderByDescending(t => t.Event.StartDate).Take(100).ToList();
            ErrorReasons = ErrorReasons.OrderByDescending(t => t.Event.StartDate).Take(100).ToList();
        }

        protected void ProcessReason(EventForRead eventObj, IStorage storage)
        {
            // пользовательские события (ошибки)
            if (eventObj.Category == EventCategory.ApplicationError || eventObj.Category == EventCategory.ComponentEvent)
            {
                var component = storage.Components.GetOneById(eventObj.OwnerId);
                var reason = new ShowEventModelReason()
                {
                    Event = eventObj,
                    EventType = storage.EventTypes.GetOneById(eventObj.EventTypeId),
                    Component = component
                };
                ErrorReasons.Add(reason);
                return;
            }

            // статус метрики
            if (eventObj.Category == EventCategory.MetricStatus)
            {
                var metric = storage.Metrics.GetOneById(eventObj.OwnerId);
                var reason = new ShowEventModelReason()
                {
                    Event = eventObj,
                    EventType = storage.EventTypes.GetOneById(eventObj.EventTypeId),
                    Metric = metric,
                    Component = storage.Components.GetOneById(metric.ComponentId)
                };
                MetricsReasons.Add(reason);
                return;
            }

            // статус юнит-теста
            if (eventObj.Category == EventCategory.UnitTestStatus)
            {
                var unitTest = storage.UnitTests.GetOneById(eventObj.OwnerId);
                var reason = new ShowEventModelReason()
                {
                    Event = eventObj,
                    EventType = storage.EventTypes.GetOneById(eventObj.EventTypeId),
                    UnitTest = unitTest,
                    Component = storage.Components.GetOneById(unitTest.ComponentId)
                };
                UnitTestsReasons.Add(reason);
                return;
            }

            // результат юнит-теста
            if (eventObj.Category == EventCategory.UnitTestResult)
            {
                var unitTest = storage.UnitTests.GetOneById(eventObj.OwnerId);
                var reason = new ShowEventModelReason()
                {
                    Event = eventObj,
                    EventType = storage.EventTypes.GetOneById(eventObj.EventTypeId),
                    UnitTest = unitTest,
                    Component = storage.Components.GetOneById(unitTest.ComponentId)
                };
                UnitTestsReasons.Add(reason);
                return;
            }

            // итоговый статус
            if (eventObj.Category == EventCategory.ComponentExternalStatus)
            {
                var component = storage.Components.GetOneById(eventObj.OwnerId);
                var reason = new ShowEventModelReason()
                {
                    Event = eventObj,
                    EventType = storage.EventTypes.GetOneById(eventObj.EventTypeId),
                    Component = component
                };
                ChildsReasons.Add(reason);
                return;
            }

            // для статусов загрузим причины еще раз
            LoadReasons(eventObj, storage);
        }

        protected void LoadReasons(EventForRead eventObj, IStorage storage)
        {
            var reasons = storage.Events.GetEventReasons(eventObj.Id).OrderByDescending(t => t.StartDate).Take(MaxEventsReasonCount).ToArray();

            foreach (var reasonsEvent in reasons)
            {
                ProcessReason(reasonsEvent, storage);
            }
        }

        public bool CanChangeImportance { get; set; }

        public bool CanChangeActuality { get; set; }

        public DateTime? GetRealEndDate(EventForRead eventObj)
        {
            var realEndDate = eventObj.StartDate + EventHelper.GetDuration(eventObj.StartDate, eventObj.ActualDate, DateTime.UtcNow);
            return eventObj.ActualDate == DateTimeHelper.InfiniteActualDate
                ? null
                : realEndDate;
        }
    }
}