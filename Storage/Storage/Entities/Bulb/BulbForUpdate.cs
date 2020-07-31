using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Описывает текущее состояние колбаски
    /// </summary>
    public class BulbForUpdate
    {
        public BulbForUpdate(Guid id)
        {
            Id = id;
            ComponentId = new ChangeTracker<Guid?>();
            MetricId = new ChangeTracker<Guid?>();
            UnitTestId = new ChangeTracker<Guid?>();
            StatusEventId = new ChangeTracker<Guid>();
            ActualDate = new ChangeTracker<DateTime>();
            FirstEventId = new ChangeTracker<Guid?>();
            HasSignal = new ChangeTracker<bool>();
            IsDeleted = new ChangeTracker<bool>();
            LastChildBulbId = new ChangeTracker<Guid?>();
            LastEventId = new ChangeTracker<Guid?>();
            Message = new ChangeTracker<string>();
            Status = new ChangeTracker<MonitoringStatus>();
            PreviousStatus = new ChangeTracker<MonitoringStatus>();
            StartDate = new ChangeTracker<DateTime>();
            EndDate = new ChangeTracker<DateTime>();
            Count = new ChangeTracker<int>();
        }

        public Guid Id { get; }

        public ChangeTracker<Guid?> ComponentId { get; }

        public ChangeTracker<Guid?> MetricId { get; }

        public ChangeTracker<Guid?> UnitTestId { get; }

        public ChangeTracker<Guid> StatusEventId { get; }

        public ChangeTracker<bool> IsDeleted { get; }

        /// <summary>
        /// Ссылка на последний дочерний компонент, который обновил данный статус
        /// </summary>
        public ChangeTracker<Guid?> LastChildBulbId { get; }

        /// <summary>
        /// Последнее событие, которое привело к данному статусу (нужно, чтобы пересчитывать статус, когда LastEvent меняет важность)
        /// </summary>
        public ChangeTracker<Guid?> LastEventId { get; }

        public ChangeTracker<string> Message { get; }

        public ChangeTracker<MonitoringStatus> Status { get; }

        public ChangeTracker<MonitoringStatus> PreviousStatus { get; }

        /// <summary>
        /// Время начала статуса
        /// </summary>
        public ChangeTracker<DateTime> StartDate { get; }

        /// <summary>
        /// Фиксированное время завершения статуса, новые статусы могут начаться только позже данной даты
        /// </summary>
        public ChangeTracker<DateTime> EndDate { get; }

        public ChangeTracker<int> Count { get; }

        public ChangeTracker<DateTime> ActualDate { get; }

        /// <summary>
        /// Первое событие, которое привело к данному статусу
        /// </summary>
        public ChangeTracker<Guid?> FirstEventId { get; }

        /// <summary>
        /// Предыдущий сигнал просрочен
        /// </summary>
        public ChangeTracker<bool> HasSignal { get; }

    }
}