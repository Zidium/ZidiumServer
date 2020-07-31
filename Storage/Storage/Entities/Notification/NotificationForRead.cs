using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Уведомление
    /// </summary>
    public class NotificationForRead
    {
        public NotificationForRead(
            Guid id, 
            Guid eventId, 
            Guid userId, 
            SubscriptionChannel type, 
            NotificationStatus status, 
            string sendError, 
            DateTime creationDate, 
            DateTime? sendDate, 
            Guid? subscriptionId, 
            NotificationReason reason, 
            string address, 
            Guid? sendEmailCommandId, 
            Guid? sendMessageCommandId)
        {
            Id = id;
            EventId = eventId;
            UserId = userId;
            Type = type;
            Status = status;
            SendError = sendError;
            CreationDate = creationDate;
            SendDate = sendDate;
            SubscriptionId = subscriptionId;
            Reason = reason;
            Address = address;
            SendEmailCommandId = sendEmailCommandId;
            SendMessageCommandId = sendMessageCommandId;
        }

        /// <summary>
        /// Id Уведомления
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Ссылка на событие
        /// </summary>
        public Guid EventId { get; }

        /// <summary>
        /// Ссылка на пользователя
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// Канал отправки
        /// </summary>
        public SubscriptionChannel Type { get; }

        /// <summary>
        /// Статус отправки
        /// </summary>
        public NotificationStatus Status { get; }

        /// <summary>
        /// Текст ошибки отправки, если есть
        /// </summary>
        public string SendError { get; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreationDate { get; }

        /// <summary>
        /// Дата отправки
        /// </summary>
        public DateTime? SendDate { get; }

        /// <summary>
        /// Ссылка на подписку, по которой создано уведомление
        /// </summary>
        public Guid? SubscriptionId { get; }

        /// <summary>
        /// Причина уведомления: 
        /// - новый важный статус
        /// - напоминание
        /// - уведомление об улучшениях
        /// </summary>
        public NotificationReason Reason { get; }

        /// <summary>
        /// Адрес доставки уведомления
        /// </summary>
        public string Address { get; }

        public Guid? SendEmailCommandId { get; }

        public Guid? SendMessageCommandId { get; }

        public NotificationForUpdate GetForUpdate()
        {
            return new NotificationForUpdate(Id);
        }

    }
}
