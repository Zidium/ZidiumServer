using System;
using Zidium.Api.Dto;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Описывает текущее состояние колбаски
    /// </summary>
    public class DbBulb
    {
        public Guid Id { get; set; }

        public Guid? ComponentId { get; set; }

        public virtual DbComponent Component { get; set; }

        public Guid? UnitTestId { get; set; }

        public virtual DbUnitTest UnitTest { get; set; }

        public Guid? MetricId { get; set; }

        public virtual DbMetric Metric { get; set; }

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

        /// <summary>
        /// Ссылка на соответствующее событие статуса
        /// </summary>
        public Guid StatusEventId { get; set; }

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

    }
}
