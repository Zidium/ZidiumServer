using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Models.Events;

namespace Zidium.UserAccount.Models
{
    public class ShowEventModel
    {
        protected AccountDbContext AccountDbContext { get; set; }

        public Event Event { get; set; }

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

        public string GetEventMessageText(Event eventObj)
        {
            if (string.IsNullOrEmpty(eventObj.Message))
            {
                return eventObj.EventType.DisplayName;
            }
            return eventObj.Message;
        }

        public EventType EventType { get; set; }

        public Component Component { get; set; }

        public Metric Metric { get; set; }

        public UnitTest UnitTest { get; set; }

        protected string GetPageTitle(EventCategory category, EventType eventType)
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

        public void Init(Guid eventId, UserInfo currentUser, AccountDbContext accountDbContext)
        {
            AccountDbContext = accountDbContext;

            ErrorReasons = new List<ShowEventModelReason>();
            UnitTestsReasons = new List<ShowEventModelReason>();
            MetricsReasons = new List<ShowEventModelReason>();
            ChildsReasons = new List<ShowEventModelReason>();

            var eventRepository = accountDbContext.GetEventRepository();
            Event = eventRepository.GetById(eventId);

            // тип события
            EventType = Event.EventType;

            // событие метрики
            if (Event.Category == EventCategory.MetricStatus)
            {
                var metricRepository = accountDbContext.GetMetricRepository();
                var metric = metricRepository.GetById(Event.OwnerId);
                Metric = metric;
                Component = metric.Component;
            }
            // событие юнит-теста
            else if (Event.Category == EventCategory.UnitTestStatus ||
                     Event.Category == EventCategory.UnitTestResult)
            {
                var unitTestRepository = accountDbContext.GetUnitTestRepository();
                var unitTest = unitTestRepository.GetById(Event.OwnerId);
                UnitTest = unitTest;
                Component = unitTest.Component;
            }
            else // событие компонента
            {
                var componentRepository = accountDbContext.GetComponentRepository();
                Component = componentRepository.GetById(Event.OwnerId);
            }

            // название страницы
            PageTitle = "Событие - " + GetPageTitle(Event.Category, EventType).ToLowerInvariant();

            // причины статусов
            if (Event.Category.IsStatus())
            {
                LoadReasons(Event, accountDbContext);
            }

            CanChangeImportance = !EventType.IsSystem;
            CanChangeActuality = Event.Category.IsComponentCategory() && !Event.Category.IsStatus() && (Event.ActualDate > DateTime.Now);

            // Оставим только 100 последних причин, иначе они долго рассчитываются и выводятся
            UnitTestsReasons = UnitTestsReasons.OrderByDescending(t => t.Event.StartDate).Take(100).ToList();
            MetricsReasons = MetricsReasons.OrderByDescending(t => t.Event.StartDate).Take(100).ToList();
            ChildsReasons = ChildsReasons.OrderByDescending(t => t.Event.StartDate).Take(100).ToList();
            ErrorReasons = ErrorReasons.OrderByDescending(t => t.Event.StartDate).Take(100).ToList();
        }

        protected void ProcessReason(Event eventObj, AccountDbContext accountDbContext)
        {
            // пользовательские события (ошибки)
            if (eventObj.Category == EventCategory.ApplicationError || eventObj.Category == EventCategory.ComponentEvent)
            {
                var component = accountDbContext.Components.First(x => x.Id == eventObj.OwnerId);
                var reason = new ShowEventModelReason()
                {
                    Event = eventObj,
                    Component = component
                };
                ErrorReasons.Add(reason);
                return;
            }

            // статус метрики
            if (eventObj.Category == EventCategory.MetricStatus)
            {
                var metric = accountDbContext.Metrics.First(x => x.Id == eventObj.OwnerId);
                var reason = new ShowEventModelReason()
                {
                    Event = eventObj,
                    Metric = metric,
                    Component = metric.Component
                };
                MetricsReasons.Add(reason);
                return;
            }

            // статус юнит-теста
            if (eventObj.Category == EventCategory.UnitTestStatus)
            {
                var unitTest = accountDbContext.UnitTests.First(x => x.Id == eventObj.OwnerId);
                var reason = new ShowEventModelReason()
                {
                    Event = eventObj,
                    UnitTest = unitTest,
                    Component = unitTest.Component
                };
                UnitTestsReasons.Add(reason);
                return;
            }

            // результат юнит-теста
            if (eventObj.Category == EventCategory.UnitTestResult)
            {
                var unitTest = accountDbContext.UnitTests.First(x => x.Id == eventObj.OwnerId);
                var reason = new ShowEventModelReason()
                {
                    Event = eventObj,
                    UnitTest = unitTest,
                    Component = unitTest.Component
                };
                UnitTestsReasons.Add(reason);
                return;
            }

            // внешняя колбаса
            if (eventObj.Category == EventCategory.ComponentExternalStatus)
            {
                var component = accountDbContext.Components.First(x => x.Id == eventObj.OwnerId);
                var reason = new ShowEventModelReason()
                {
                    Event = eventObj,
                    Component = component
                };
                ChildsReasons.Add(reason);
                return;
            }

            // для статусов загрузим причины еще раз
            LoadReasons(eventObj, accountDbContext);
        }

        protected void LoadReasons(Event eventObj, AccountDbContext accountDbContext)
        {
            var eventRepository = accountDbContext.GetEventRepository();
            var reasons = eventRepository.GetEventReasons(eventObj.Id).OrderByDescending(t => t.StartDate).Take(MaxEventsReasonCount).ToArray();

            foreach (var reasonsEvent in reasons)
            {
                ProcessReason(reasonsEvent, accountDbContext);
            }
        }

        public bool CanChangeImportance { get; set; }

        public bool CanChangeActuality { get; set; }
    }
}