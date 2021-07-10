using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    /// <summary>
    /// Подписка на уведомление о событии
    /// </summary>
    public class SubscriptionForRead
    {
        public SubscriptionForRead(
            Guid id, 
            Guid userId, 
            Guid? componentTypeId, 
            Guid? componentId, 
            SubscriptionObject subscriptionObject, 
            SubscriptionChannel channel, 
            bool isEnabled, 
            bool notifyBetterStatus, 
            EventImportance importance, 
            int? durationMinimumInSeconds, 
            int? resendTimeInSeconds, 
            DateTime lastUpdated, 
            bool sendOnlyInInterval, 
            int? sendIntervalFromHour, 
            int? sendIntervalFromMinute, 
            int? sendIntervalToHour, 
            int? sendIntervalToMinute)
        {
            Id = id;
            UserId = userId;
            ComponentTypeId = componentTypeId;
            ComponentId = componentId;
            Object = subscriptionObject;
            Channel = channel;
            IsEnabled = isEnabled;
            NotifyBetterStatus = notifyBetterStatus;
            Importance = importance;
            DurationMinimumInSeconds = durationMinimumInSeconds;
            ResendTimeInSeconds = resendTimeInSeconds;
            LastUpdated = lastUpdated;
            SendOnlyInInterval = sendOnlyInInterval;
            SendIntervalFromHour = sendIntervalFromHour;
            SendIntervalFromMinute = sendIntervalFromMinute;
            SendIntervalToHour = sendIntervalToHour;
            SendIntervalToMinute = sendIntervalToMinute;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Ссылка на пользователя
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// Ссылка на тип компонента
        /// </summary>
        public Guid? ComponentTypeId { get; }

        public Guid? ComponentId { get; }

        /// <summary>
        /// Объект подписки (на что подписываемся)
        /// </summary>
        public SubscriptionObject Object { get; }

        /// <summary>
        /// Канал отправки уведомлений
        /// </summary>
        public SubscriptionChannel Channel { get; }

        /// <summary>
        /// Подписка включена?
        /// </summary>
        public bool IsEnabled { get; }

        /// <summary>
        /// Флажок, нужно ли отправлять уведомление, что стало лучше
        /// </summary>
        public bool NotifyBetterStatus { get; }

        /// <summary>
        /// Минимальная важность
        /// </summary>
        public EventImportance Importance { get; }
        
        /// <summary>
        /// Минимальная длительность события, сек
        /// </summary>
        public int? DurationMinimumInSeconds { get; }

        /// <summary>
        /// Интервал повторной отправки уведомления, сек
        /// </summary>
        public int? ResendTimeInSeconds { get; }

        /// <summary>
        /// Дата и время последнего изменения
        /// </summary>
        public DateTime LastUpdated { get; }

        /// <summary>
        /// Отправлять только в указанный интервал 
        /// </summary>
        public bool SendOnlyInInterval { get; }

        /// <summary>
        /// Отправлять не раньше этого часа
        /// </summary>
        public int? SendIntervalFromHour { get; }

        /// <summary>
        /// Отправлять не раньше этой минуты
        /// </summary>
        public int? SendIntervalFromMinute { get; }

        /// <summary>
        /// Отправлять не позже этого часа
        /// </summary>
        public int? SendIntervalToHour { get; }

        /// <summary>
        /// Отправлять не позже этой минуты
        /// </summary>
        public int? SendIntervalToMinute { get; }

        public SubscriptionForUpdate GetForUpdate()
        {
            return new SubscriptionForUpdate(Id);
        }

    }
}
