using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public class SubscriptionForAdd
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id;

        /// <summary>
        /// Ссылка на пользователя
        /// </summary>
        public Guid UserId;

        /// <summary>
        /// Ссылка на тип компонента
        /// </summary>
        public Guid? ComponentTypeId;

        public Guid? ComponentId;

        /// <summary>
        /// Объект подписки (на что подписываемся)
        /// </summary>
        public SubscriptionObject Object;

        /// <summary>
        /// Канал отправки уведомлений
        /// </summary>
        public SubscriptionChannel Channel;

        /// <summary>
        /// Подписка включена?
        /// </summary>
        public bool IsEnabled;

        /// <summary>
        /// Флажок, нужно ли отправлять уведомление, что стало лучше
        /// </summary>
        public bool NotifyBetterStatus;

        /// <summary>
        /// Минимальная важность
        /// </summary>
        public EventImportance Importance;

        /// <summary>
        /// Минимальная длительность события, сек
        /// </summary>
        public int? DurationMinimumInSeconds;

        /// <summary>
        /// Интервал повторной отправки уведомления, сек
        /// </summary>
        public int? ResendTimeInSeconds;

        /// <summary>
        /// Дата и время последнего изменения
        /// </summary>
        public DateTime LastUpdated;

        /// <summary>
        /// Отправлять только в указанный интервал 
        /// </summary>
        public bool SendOnlyInInterval;

        /// <summary>
        /// Отправлять не раньше этого часа
        /// </summary>
        public int? SendIntervalFromHour;

        /// <summary>
        /// Отправлять не раньше этой минуты
        /// </summary>
        public int? SendIntervalFromMinute;

        /// <summary>
        /// Отправлять не позже этого часа
        /// </summary>
        public int? SendIntervalToHour;

        /// <summary>
        /// Отправлять не позже этой минуты
        /// </summary>
        public int? SendIntervalToMinute;

    }
}
