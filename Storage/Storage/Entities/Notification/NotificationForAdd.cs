using System;

namespace Zidium.Storage
{
    public class NotificationForAdd
    {
        /// <summary>
        /// Id Уведомления
        /// </summary>
        public Guid Id;

        /// <summary>
        /// Ссылка на событие
        /// </summary>
        public Guid EventId;

        /// <summary>
        /// Ссылка на пользователя
        /// </summary>
        public Guid UserId;

        /// <summary>
        /// Канал отправки
        /// </summary>
        public SubscriptionChannel Type;

        /// <summary>
        /// Статус отправки
        /// </summary>
        public NotificationStatus Status;

        /// <summary>
        /// Текст ошибки отправки, если есть
        /// </summary>
        public string SendError;

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreationDate;

        /// <summary>
        /// Дата отправки
        /// </summary>
        public DateTime? SendDate;

        /// <summary>
        /// Ссылка на подписку, по которой создано уведомление
        /// </summary>
        public Guid? SubscriptionId;

        /// <summary>
        /// Причина уведомления: 
        /// - новый важный статус
        /// - напоминание
        /// - уведомление об улучшениях
        /// </summary>
        public NotificationReason Reason;

        /// <summary>
        /// Адрес доставки уведомления
        /// </summary>
        public string Address;

        public Guid? SendEmailCommandId;

        public Guid? SendMessageCommandId;
    }
}
