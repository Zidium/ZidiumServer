using System;
using System.Collections.Generic;
using Zidium.Core;
using Zidium.Storage;
using Zidium.UserAccount.Models.Controls;

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

        public UnitTestForRead UnitTest { get; set; }

        public MetricForRead Metric { get; set; }

        public string VersionFrom { get; set; }

        public IStorage Storage { get; set; }

        public EventsListItemModel[] Events { get; set; }

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

        public Dictionary<Guid, EventsListComponentModel> Components { get; set; }

        public Dictionary<Guid, EventsListEventTypeModel> EventTypes { get; set; }

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

        public static readonly int MaxMessageLength = 150;

    }
}