using System;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Уведомление
    /// </summary>
    public class DbNotification
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
        public virtual DbEvent Event { get; set; }

        /// <summary>
        /// Ссылка на пользователя
        /// </summary>
        public Guid UserId { get; set; }

        public virtual DbUser User { get; set; }

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

        public virtual DbSubscription Subscription { get; set; }

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
        public virtual DbNotificationHttp NotificationHttp { get; set; }

        public Guid? SendEmailCommandId { get; set; }

        public virtual DbSendEmailCommand SendEmailCommand { get; set; }

        public Guid? SendMessageCommandId { get; set; }

        public virtual DbSendMessageCommand SendMessageCommand { get; set; }

    }
}
