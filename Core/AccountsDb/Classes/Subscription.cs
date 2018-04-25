using System;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Подписка на уведомление о событии
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Ссылка на пользователя
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Ссылка на тип компонента
        /// </summary>
        public Guid? ComponentTypeId { get; set; }

        /// <summary>
        /// Тип компонента
        /// </summary>
        public virtual ComponentType ComponentType { get; set; }

        public Guid? ComponentId { get; set; }

        public virtual Component Component { get; set; }

        /// <summary>
        /// Объект подписки (на что подписываемся)
        /// </summary>
        public SubscriptionObject Object { get; set; }

        /// <summary>
        /// Канал отправки уведомлений
        /// </summary>
        public SubscriptionChannel Channel { get; set; }

        /// <summary>
        /// Подписка включена?
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Флажок, нужно ли отправлять уведомление, что стало лучше
        /// </summary>
        public bool NotifyBetterStatus { get; set; }

        /// <summary>
        /// Минимальная важность
        /// </summary>
        public EventImportance Importance { get; set; }
        
        /// <summary>
        /// Минимальная длительность события, сек
        /// </summary>
        public int? DurationMinimumInSeconds { get; set; }

        /// <summary>
        /// Интервал повторной отправки уведомления, сек
        /// </summary>
        public int? ResendTimeInSeconds { get; set; }

        /// <summary>
        /// Дата и время последнего изменения
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }
}
