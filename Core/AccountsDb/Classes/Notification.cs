using System;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Уведомление
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Id Уведомления
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Ссылка на событие
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Событие, для которого создано уведомление
        /// </summary>
        public virtual Event Event { get; set; }

        /// <summary>
        /// Ссылка на пользователя
        /// </summary>
        public Guid UserId { get; set; }

        public virtual User User { get; set; }

        /// <summary>
        /// Канал отправки
        /// </summary>
        public SubscriptionChannel Type { get; set; }

        /// <summary>
        /// Статус отправки
        /// </summary>
        public NotificationStatus Status { get; set; }

        /// <summary>
        /// Текст ошибки отправки, если есть
        /// </summary>
        public string SendError { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Дата отправки
        /// </summary>
        public DateTime? SendDate { get; set; }

        /// <summary>
        /// Ссылка на подписку, по которой создано уведомление
        /// </summary>
        public Guid? SubscriptionId { get; set; }

        public virtual Subscription Subscription { get; set; }

        /// <summary>
        /// Причина уведомления: 
        /// - новый важный статус
        /// - напоминание
        /// - уведомление об улучшениях
        /// </summary>
        public NotificationReason Reason { get; set; }

        /// <summary>
        /// Адрес доставки уведомления
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Http-часть уведомления, для уведомлений с типом http
        /// </summary>
        public virtual NotificationHttp NotificationHttp { get; set; }

        public Guid? SendEmailCommandId { get; set; }

        public virtual SendEmailCommand SendEmailCommand { get; set; }

        public Guid? SendMessageCommandId { get; set; }

        public virtual SendMessageCommand SendMessageCommand { get; set; }

    }
}
