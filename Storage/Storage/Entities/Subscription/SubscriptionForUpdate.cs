using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Подписка на уведомление о событии
    /// </summary>
    public class SubscriptionForUpdate
    {
        public SubscriptionForUpdate(Guid id)
        {
            Id = id;
            IsEnabled = new ChangeTracker<bool>();
            NotifyBetterStatus = new ChangeTracker<bool>();
            Importance = new ChangeTracker<EventImportance>();
            DurationMinimumInSeconds = new ChangeTracker<int?>();
            ResendTimeInSeconds = new ChangeTracker<int?>();
            LastUpdated = new ChangeTracker<DateTime>();
            SendOnlyInInterval = new ChangeTracker<bool>();
            SendIntervalFromHour = new ChangeTracker<int?>();
            SendIntervalFromMinute = new ChangeTracker<int?>();
            SendIntervalToHour = new ChangeTracker<int?>();
            SendIntervalToMinute = new ChangeTracker<int?>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Подписка включена?
        /// </summary>
        public ChangeTracker<bool> IsEnabled { get; }

        /// <summary>
        /// Флажок, нужно ли отправлять уведомление, что стало лучше
        /// </summary>
        public ChangeTracker<bool> NotifyBetterStatus { get; }

        /// <summary>
        /// Минимальная важность
        /// </summary>
        public ChangeTracker<EventImportance> Importance { get; }
        
        /// <summary>
        /// Минимальная длительность события, сек
        /// </summary>
        public ChangeTracker<int?> DurationMinimumInSeconds { get; }

        /// <summary>
        /// Интервал повторной отправки уведомления, сек
        /// </summary>
        public ChangeTracker<int?> ResendTimeInSeconds { get; }

        /// <summary>
        /// Дата и время последнего изменения
        /// </summary>
        public ChangeTracker<DateTime> LastUpdated { get; }

        /// <summary>
        /// Отправлять только в указанный интервал 
        /// </summary>
        public ChangeTracker<bool> SendOnlyInInterval { get; }

        /// <summary>
        /// Отправлять не раньше этого часа
        /// </summary>
        public ChangeTracker<int?> SendIntervalFromHour { get; }

        /// <summary>
        /// Отправлять не раньше этой минуты
        /// </summary>
        public ChangeTracker<int?> SendIntervalFromMinute { get; }

        /// <summary>
        /// Отправлять не позже этого часа
        /// </summary>
        public ChangeTracker<int?> SendIntervalToHour { get; }

        /// <summary>
        /// Отправлять не позже этой минуты
        /// </summary>
        public ChangeTracker<int?> SendIntervalToMinute { get; }

    }
}