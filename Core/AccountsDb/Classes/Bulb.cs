using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Описывает текущее состояние колбаски
    /// </summary>
    public class Bulb
    {
        public Guid Id { get; set; }

        public Guid? ComponentId { get; set; }

        public virtual Component Component { get; set; }

        public Guid? UnitTestId { get; set; }

        public virtual UnitTest UnitTest { get; set; }

        public Guid? MetricId { get; set; }

        public virtual Metric Metric { get; set; }

        public EventCategory EventCategory { get; set; }

        public MonitoringStatus Status { get; set; }

        public MonitoringStatus PreviousStatus { get; set; }

        /// <summary>
        /// Дата создания статуса
        /// CreateDate >= StartDate
        /// Если создаем пробел, то начало пробела = конец предыдущего статуса, с дата создания = текущее время
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Время начала статуса
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Фиксированное время завершения статуса, новые статусы могут начаться только позже данной даты
        /// </summary>
        public DateTime EndDate { get; set; }

        public int Count { get; set; }

        public DateTime ActualDate { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Юнит-тесты могут НЕ передавать сообщение в результате, поэтому для таких результатов сгенерируем message сами
        /// </summary>
        /// <returns></returns>
        public string GetUnitTestMessage()
        {
            if (string.IsNullOrEmpty(Message))
            {
                if (Status == MonitoringStatus.Alarm)
                {
                    return "alarm";
                }
                if (Status == MonitoringStatus.Warning)
                {
                    return "warning";
                }
                if (Status == MonitoringStatus.Success)
                {
                    return "success";
                }
                if (Status == MonitoringStatus.Unknown)
                {
                    return "unknown";
                }
                if (Status == MonitoringStatus.Disabled)
                {
                    return "disabled";
                }
            }
            return Message;
        }

        /// <summary>
        /// Первое событие, которое привело к данному статусу
        /// </summary>
        public Guid? FirstEventId { get; set; }

        /// <summary>
        /// Последнее событие, которое привело к данному статусу (нужно, чтобы пересчитывать статус, когда LastEvent меняет важность)
        /// </summary>
        public Guid? LastEventId { get; set; }

        /// <summary>
        /// Ссылка на последний дочерний компонент, который обновил данный статус
        /// </summary>
        public Guid? LastChildBulbId { get; set; }

        //public virtual StatusData LastChildStatus { get; set; }

        /// <summary>
        /// Ссылка на соответствующее событие статуса
        /// </summary>
        public Guid StatusEventId { get; set; }

        /// <summary>
        /// Длительность статуса
        /// </summary>
        public TimeSpan GetDuration(DateTime now)
        {
            if (ActualDate > now)
            {
                return now - StartDate;
            }
            return ActualDate - StartDate;
        }

        /// <summary>
        /// Время начала расчета UpTime
        /// </summary>
        public DateTime UpTimeStartDate { get; set; }

        /// <summary>
        /// Длина статистики UpTime в мс
        /// </summary>
        public long UpTimeLengthMs { get; set; }

        /// <summary>
        /// Количество исправной работы в мс
        /// </summary>
        public long UpTimeSuccessMs { get; set; }

        /// <summary>
        /// Предыдущий сигнал просрочен
        /// </summary>
        public bool HasSignal { get; set; }

        public bool IsDeleted { get; set; }

        public bool Actual(DateTime date)
        {
            return ActualDate >= date;
        }

        public Bulb GetParent()
        {
            if (EventCategory == EventCategory.UnitTestStatus)
            {
                if (UnitTest == null)
                {
                    return null;
                }
                return UnitTest.Component.UnitTestsStatus;
            }
            if (EventCategory == EventCategory.MetricStatus)
            {
                if (Metric == null)
                {
                    return null;
                }
                return Metric.Component.MetricsStatus;
            }
            if (Component == null)
            {
                return null;
            }
            if (EventCategory == EventCategory.ComponentUnitTestsStatus)
            {
                return Component.InternalStatus;
            }
            
            if (EventCategory == EventCategory.ComponentMetricsStatus)
            {
                return Component.InternalStatus;
            }
            if (EventCategory == EventCategory.ComponentEventsStatus)
            {
                return Component.InternalStatus;
            }
            if (EventCategory == EventCategory.ComponentChildsStatus)
            {
                return Component.ExternalStatus;
            }
            if (EventCategory == EventCategory.ComponentInternalStatus)
            {
                return Component.ExternalStatus;
            }
            if (EventCategory == EventCategory.ComponentExternalStatus)
            {
                if (Component.IsRoot)
                {
                    return null;
                }
                return Component.Parent.ChildComponentsStatus;
            }
            throw new Exception("Неизвестное значение EventCategory: " + EventCategory);
        }

        public List<Bulb> GetChilds()
        {
            if (EventCategory == EventCategory.UnitTestStatus)
            {
                return new List<Bulb>(0); // нет детей
            }
            if (EventCategory == EventCategory.ComponentUnitTestsStatus)
            {
                var childs = Component.UnitTests.Where(x => x.IsDeleted == false).Select(x => x.Bulb).ToList();
                return childs;
            }
            if (EventCategory == EventCategory.MetricStatus)
            {
                return new List<Bulb>(0); // нет детей
            }
            if (EventCategory == EventCategory.ComponentMetricsStatus)
            {
                var childs = Component.Metrics.Where(x => x.IsDeleted == false).Select(x => x.Bulb).ToList();
                return childs;
            }
            if (EventCategory == EventCategory.ComponentEventsStatus)
            {
                return new List<Bulb>(0); // нет детей
            }
            if (EventCategory == EventCategory.ComponentChildsStatus)
            {
                var childs = Component.Childs.Where(x => x.IsDeleted == false).Select(x => x.ExternalStatus).ToList();
                return childs;
            }
            if (EventCategory == EventCategory.ComponentInternalStatus)
            {
                return new List<Bulb>()
                {
                    Component.UnitTestsStatus,
                    Component.MetricsStatus,
                    Component.EventsStatus
                };
            }
            if (EventCategory == EventCategory.ComponentExternalStatus)
            {
                return new List<Bulb>()
                {
                    Component.InternalStatus,
                    Component.ChildComponentsStatus
                };
            }
            throw new Exception("Неизвестное значение EventCategory: " + EventCategory);
        }

        public Guid GetComponentId()
        {
            // если это колбаска проверки
            if (EventCategory == EventCategory.UnitTestStatus)
            {
                return UnitTest.ComponentId;
            }

            // если это колбаска метрики
            if (EventCategory == EventCategory.MetricStatus)
            {
                return Metric.ComponentId;
            }

            // все другие колбаски - это колбаски компонента
            return ComponentId.Value;
        }

        public Guid GetOwnerId()
        {
            if (ComponentId.HasValue)
            {
                return ComponentId.Value;
            }
            if (UnitTestId.HasValue)
            {
                return UnitTestId.Value;
            }
            if (MetricId.HasValue)
            {
                return MetricId.Value;
            }
            throw new Exception("Не удалось найти OwnerId");
        }

        /// <summary>
        /// Лист дерева статусов (НЕ имеет детей)
        /// </summary>
        public bool IsLeaf
        {
            get
            {
                return EventCategory == EventCategory.MetricStatus
                       || EventCategory == EventCategory.UnitTestStatus
                       || EventCategory == EventCategory.ComponentEventsStatus;
            }
        }
    }
}
