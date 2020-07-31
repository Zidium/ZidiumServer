using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Описывает текущее состояние колбаски
    /// </summary>
    public class BulbForRead
    {
        public BulbForRead(
            Guid id, 
            Guid? componentId, 
            Guid? unitTestId, 
            Guid? metricId, 
            EventCategory eventCategory, 
            MonitoringStatus status, 
            MonitoringStatus previousStatus, 
            DateTime createDate, 
            DateTime startDate, 
            DateTime endDate, 
            int count, 
            DateTime actualDate, 
            string message, 
            Guid? firstEventId, 
            Guid? lastEventId, 
            Guid? lastChildBulbId, 
            Guid statusEventId, 
            DateTime upTimeStartDate, 
            long upTimeLengthMs, 
            long upTimeSuccessMs, 
            bool hasSignal, 
            bool isDeleted)
        {
            Id = id;
            ComponentId = componentId;
            UnitTestId = unitTestId;
            MetricId = metricId;
            EventCategory = eventCategory;
            Status = status;
            PreviousStatus = previousStatus;
            CreateDate = createDate;
            StartDate = startDate;
            EndDate = endDate;
            Count = count;
            ActualDate = actualDate;
            Message = message;
            FirstEventId = firstEventId;
            LastEventId = lastEventId;
            LastChildBulbId = lastChildBulbId;
            StatusEventId = statusEventId;
            UpTimeStartDate = upTimeStartDate;
            UpTimeLengthMs = upTimeLengthMs;
            UpTimeSuccessMs = upTimeSuccessMs;
            HasSignal = hasSignal;
            IsDeleted = isDeleted;
        }

        public Guid Id { get; }

        public Guid? ComponentId { get; }

        public Guid? UnitTestId { get; }

        public Guid? MetricId { get; }

        public EventCategory EventCategory { get; }

        public MonitoringStatus Status { get; }

        public MonitoringStatus PreviousStatus { get; }

        /// <summary>
        /// Дата создания статуса
        /// CreateDate >= StartDate
        /// Если создаем пробел, то начало пробела = конец предыдущего статуса, с дата создания = текущее время
        /// </summary>
        public DateTime CreateDate { get; }

        /// <summary>
        /// Время начала статуса
        /// </summary>
        public DateTime StartDate { get; }

        /// <summary>
        /// Фиксированное время завершения статуса, новые статусы могут начаться только позже данной даты
        /// </summary>
        public DateTime EndDate { get; }

        public int Count { get; }

        public DateTime ActualDate { get; }

        public string Message { get; }

        /// <summary>
        /// Первое событие, которое привело к данному статусу
        /// </summary>
        public Guid? FirstEventId { get; }

        /// <summary>
        /// Последнее событие, которое привело к данному статусу (нужно, чтобы пересчитывать статус, когда LastEvent меняет важность)
        /// </summary>
        public Guid? LastEventId { get; }

        /// <summary>
        /// Ссылка на последний дочерний компонент, который обновил данный статус
        /// </summary>
        public Guid? LastChildBulbId { get; }

        //public virtual StatusData LastChildStatus { get; }

        /// <summary>
        /// Ссылка на соответствующее событие статуса
        /// </summary>
        public Guid StatusEventId { get; }

        /// <summary>
        /// Время начала расчета UpTime
        /// </summary>
        public DateTime UpTimeStartDate { get; }

        /// <summary>
        /// Длина статистики UpTime в мс
        /// </summary>
        public long UpTimeLengthMs { get; }

        /// <summary>
        /// Количество исправной работы в мс
        /// </summary>
        public long UpTimeSuccessMs { get; }

        /// <summary>
        /// Предыдущий сигнал просрочен
        /// </summary>
        public bool HasSignal { get; }

        public bool IsDeleted { get; }

        public BulbForUpdate GetForUpdate()
        {
            return new BulbForUpdate(Id);
        }
    }
}
