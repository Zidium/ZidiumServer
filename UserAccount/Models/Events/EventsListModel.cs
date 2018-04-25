using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.UserAccount.Models.Controls;
using Zidium.UserAccount.Models.Events;

namespace Zidium.UserAccount.Models
{
    public class EventsListModel
    {
        public Guid? ComponentId { get; set; }

        public Guid? ComponentTypeId { get; set; }

        public ColorStatusSelectorValue Color { get; set; }

        public Guid? EventTypeId { get; set; }

        public EventCategory? Category { get; set; }

        public string Search { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public UnitTest UnitTest { get; set; }

        public Metric Metric { get; set; }

        public string VersionFrom { get; set; }

        public IQueryable<EventsListItemModel> Events { get; set; }

        /// <summary>
        /// Показывать колбасу? Показываем, если явно указан фильтр по категории какого-либо статуса
        /// </summary>
        public bool ShowTimeline
        {
            get { return Category.HasValue && Category.Value.IsStatus() && 
                    (ComponentId.HasValue || UnitTest != null || Metric != null || EventTypeId.HasValue); }
        }

        public string GetPageTitle()
        {
            if (UnitTest != null)
            {
                return "События проверки";
            }
            if (Metric != null)
            {
                return "События метрики";
            }
            return "События";
        }

        /// <summary>
        /// Показываем события компонентов
        /// </summary>
        public bool IsComponentEventsMode
        {
            get
            {
                // если проверка и метрика не указаны, значит это события компонентов
                return UnitTest == null && Metric == null;
            }
        }

        public EventXmlData[] GetXmlEvents(AccountDbContext accountDbContext)
        {
            // загрузим все свойства
            var eventIdArray = Events.Select(x => x.Id).ToArray();

            var allProperties = accountDbContext.EventProperties
                .Where(x => eventIdArray.Contains(x.EventId))
                .ToArray();

            var eventPropertiesGroups = allProperties
                .GroupBy(x => x.EventId)
                .ToDictionary(x => x.Key, x=>x.ToArray());

            var events = new List<EventXmlData>();
            foreach (var item in Events)
            {
                // тип события
                EventsListEventTypeModel eventType = null;
                EventTypes.TryGetValue(item.EventTypeId, out eventType);
                if (eventType == null)
                {
                    continue;
                }

                // компонент
                EventsListComponentModel component = null;
                Components.TryGetValue(item.OwnerId, out component);
                if (component == null)
                {
                    continue;
                }

                var eventObj = new EventXmlData()
                {
                    Id = item.Id,
                    EventTypeId = eventType.Id,
                    Count = item.Count,
                    ActualDate = item.ActualDate,
                    EndDate = item.EndDate,
                    EventTypeDisplayName = eventType.DisplayName,
                    EventTypeSystemName = eventType.SystemName,
                    JoinKeyHash = item.JoinKey,
                    Message = item.Message,
                    Category = item.Category.ToString(),
                    StartDate = item.StartDate,
                    TypeCode = eventType.Code,
                    OwnerId = item.OwnerId,
                    ComponentId = component.Id,
                    ComponentSystemName = component.SystemName,
                    ComponentDisplayName = component.DisplayName
                };

                EventProperty[] eventProperties = null;
                if (eventPropertiesGroups.TryGetValue(eventObj.Id, out eventProperties))
                {
                    eventObj.Properties = eventProperties.Select(x => new EventPropertyXml()
                    {
                        Key = x.Name,
                        Type = x.DataType.ToString(),
                        Vaue = x.Value
                    }).ToArray();
                }
                events.Add(eventObj);
            }
            return events.ToArray();
        }

        public EventCategory[] GetEventCategoriesForFilter()
        {
            // категории компонента
            if (IsComponentEventsMode)
            {
                return EventCategoryHelper.ComponentCategories;
            }

            // категории проверки
            if (UnitTest != null)
            {
                return new[]
                {
                    EventCategory.UnitTestResult, 
                    EventCategory.UnitTestStatus
                };
            }

            // у метрики только одна категория
            return null;
        }

        public Dictionary<Guid, EventsListComponentModel> Components { get; set; }

        public Dictionary<Guid, EventsListEventTypeModel> EventTypes { get; set; }

        public static readonly int MaxMessageLength = 150;
        public static readonly int MaxPropertyLength = 4000;
    }
}