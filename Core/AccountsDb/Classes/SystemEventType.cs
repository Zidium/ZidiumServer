using System;
using System.Linq;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    public class SystemEventType
    {
        #region Свойства экземпляра

        public Guid Id { get; set; }

        public EventCategory Category { get; set; }

        /// <summary>
        /// Отображаемое название типа события
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Внутреннее название типа события
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Интервал склейки - обертка над свойством JoinIntervalSeconds
        /// </summary>
        public TimeSpan? JoinInterval { get; set; }

        /// <summary>
        /// Важность
        /// </summary>
        public EventImportance Importance { get; set; }

        public EventType GetEventType()
        {
            return new EventType()
            {
                Id = Id,
                Category = Category,
                ImportanceForNew = Importance,
                DisplayName = DisplayName,
                SystemName = SystemName,
                JoinInterval = JoinInterval
            };
        }

        #endregion

        public static SystemEventType GetStatusEventType(EventCategory category)
        {
            if (category == EventCategory.ComponentInternalStatus)
            {
                return ComponentInternalStatus;
            }
            if (category == EventCategory.ComponentExternalStatus)
            {
                return ComponentExternalStatus;
            }
            if (category == EventCategory.ComponentEventsStatus)
            {
                return ComponentEventsStatus;
            }
            if (category == EventCategory.ComponentUnitTestsStatus)
            {
                return ComponentUnitTestsStatus; 
            }
            if (category == EventCategory.ComponentMetricsStatus)
            {
                return ComponentMetricsStatus;
            }
            if (category == EventCategory.ComponentChildsStatus)
            {
                return ComponentChildsStatus;
            }
            if (category == EventCategory.UnitTestResult)
            {
                return UnitTestResult;
            }
            if (category == EventCategory.UnitTestStatus)
            {
                return UnitTestStatus;
            }
            //if (category == EventCategory.MetricResult)
            //{
            //    return MetricResult;
            //}
            if (category == EventCategory.MetricStatus)
            {
                return MetricStatus;
            }
            throw new Exception("Неизвестное значение EventCategory: " + category);
        }

        /// <summary>
        /// События внутреннего статуса компонента
        /// </summary>
        public static readonly SystemEventType ComponentInternalStatus = new SystemEventType()
        {
            Id = new Guid("4EA3B90B-1620-4AA5-A7F6-D924C6D78C68"),
            Category = EventCategory.ComponentInternalStatus,
            Importance = EventImportance.Unknown,
            DisplayName = "Внутренний статус компонента",
            SystemName = "System.EventTypes.ComponentInternalStatus",
            JoinInterval = TimeSpan.FromHours(1)
        };

        /// <summary>
        /// События внешнего статуса компонента
        /// </summary>
        public static readonly SystemEventType ComponentExternalStatus = new SystemEventType()
        {
            Id = new Guid("788D4A44-94FB-4051-8234-24418399AEB9"),
            Category = EventCategory.ComponentExternalStatus,
            Importance = EventImportance.Unknown,
            DisplayName = "Итоговый статус компонента",
            SystemName = "System.EventTypes.ComponentExternalStatus",
            JoinInterval = TimeSpan.FromHours(1)
        };

        /// <summary>
        /// Колбаска событий компонента
        /// </summary>
        public static readonly SystemEventType ComponentEventsStatus = new SystemEventType()
        {
            Id = new Guid("EE6A772E-43DD-48FC-B5A8-68E5D6690440"),
            Category = EventCategory.ComponentEventsStatus,
            Importance = EventImportance.Unknown,
            DisplayName = "Статус событий компонента",
            SystemName = "System.EventTypes.ComponentEventsStatus",
            JoinInterval = TimeSpan.FromHours(1)
        };

        /// <summary>
        /// Колбаска юнит-тестов компонента
        /// </summary>
        public static readonly SystemEventType ComponentUnitTestsStatus = new SystemEventType()
        {
            Id = new Guid("B2B3FECD-DAB9-4383-B0C9-4C1C5D455AC1"),
            Category = EventCategory.ComponentUnitTestsStatus,
            Importance = EventImportance.Unknown,
            DisplayName = "Статус проверок компонента",
            SystemName = "System.EventTypes.ComponentUnitTestsStatus",
            JoinInterval = TimeSpan.FromHours(1)
        };

        /// <summary>
        /// Колбаска метрик компонента
        /// </summary>
        public static readonly SystemEventType ComponentMetricsStatus = new SystemEventType()
        {
            Id = new Guid("F69A6245-5253-4C27-94CC-327FEA5DA448"),
            Category = EventCategory.ComponentMetricsStatus,
            Importance = EventImportance.Unknown,
            DisplayName = "Статус метрик компонента",
            SystemName = "System.EventTypes.ComponentMetricsStatus",
            JoinInterval = TimeSpan.FromHours(1)
        };

        /// <summary>
        /// Колбаска дочерних компонента
        /// </summary>
        public static readonly SystemEventType ComponentChildsStatus = new SystemEventType()
        {
            Id = new Guid("47541379-592A-40F1-897F-8A134F5E5FC5"),
            Category = EventCategory.ComponentChildsStatus,
            Importance = EventImportance.Unknown,
            DisplayName = "Статус дочерних компонентов",
            SystemName = "System.EventTypes.ComponentChildsStatus",
            JoinInterval = TimeSpan.FromHours(1)
        };

        /// <summary>
        /// Статус проверки
        /// </summary>
        public static readonly SystemEventType UnitTestStatus = new SystemEventType()
        {
            Id = new Guid("B2AF8EC5-3F2A-4751-8C29-9C00248B0702"),
            Category = EventCategory.UnitTestStatus,
            Importance = EventImportance.Unknown,
            DisplayName = "Статус проверки",
            SystemName = "System.EventTypes.UnitTestStatus",
            JoinInterval = TimeSpan.FromHours(1)
        };

        /// <summary>
        /// Результат проверки
        /// </summary>
        public static readonly SystemEventType UnitTestResult = new SystemEventType()
        {
            Id = new Guid("FCCF8630-16EF-4B73-9E95-D7F4593D6472"),
            Category = EventCategory.UnitTestResult,
            Importance = EventImportance.Unknown,
            DisplayName = "Результат проверки",
            SystemName = "System.EventTypes.UnitTestResult",
            JoinInterval = TimeSpan.FromHours(1)
        };

        /// <summary>
        /// Запуск компонента
        /// </summary>
        public const string ComponentStart = "ComponentStart";

        /// <summary>
        /// Статус метрики
        /// </summary>
        public static readonly SystemEventType MetricStatus = new SystemEventType()
        {
            Id = new Guid("12415C6F-D858-4C57-9512-0B57EE361695"),
            Category = EventCategory.MetricStatus,
            Importance = EventImportance.Unknown,
            DisplayName = "Статус метрики",
            SystemName = "System.EventTypes.MetricStatus",
            JoinInterval = TimeSpan.FromHours(1)
        };

        ///// <summary>
        ///// Результат метрики
        ///// </summary>
        //public static readonly SystemEventType MetricResult = new SystemEventType()
        //{
        //    Id = new Guid("C05D13F4-712C-4645-8666-CA815C32165D"),
        //    Category = EventCategory.MetricResult,
        //    Importance = EventImportance.Unknown,
        //    DisplayName = "Результат метрики",
        //    SystemName = "System.EventTypes.MetricResult",
        //    JoinInterval = TimeSpan.FromHours(1)
        //};

        public SystemEventType[] GetAll()
        {
            return new[]
            {
                ComponentEventsStatus,
                ComponentUnitTestsStatus,
                ComponentMetricsStatus,
                ComponentChildsStatus,
                ComponentInternalStatus,
                ComponentExternalStatus,
                UnitTestResult,
                UnitTestStatus,
                MetricStatus
            };
        }

        public bool IsSystem(Guid id)
        {
            var all = GetAll();
            return all.Any(x => x.Id == id);
        }
    }
}
