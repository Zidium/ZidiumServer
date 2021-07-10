using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Уведомление
    /// </summary>
    public class NotificationForUpdate
    {
        public NotificationForUpdate(Guid id)
        {
            Id = id;
            Status = new ChangeTracker<NotificationStatus>();
            SendError = new ChangeTracker<string>();
            SendDate = new ChangeTracker<DateTime?>();
            SendEmailCommandId = new ChangeTracker<Guid?>();
            SendMessageCommandId = new ChangeTracker<Guid?>();
        }

        /// <summary>
        /// Id Уведомления
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Статус отправки
        /// </summary>
        public ChangeTracker<NotificationStatus> Status { get; }

        /// <summary>
        /// Текст ошибки отправки, если есть
        /// </summary>
        public ChangeTracker<string> SendError { get; }

        /// <summary>
        /// Дата отправки
        /// </summary>
        public ChangeTracker<DateTime?> SendDate { get; }

        public ChangeTracker<Guid?> SendEmailCommandId { get; }

        public ChangeTracker<Guid?> SendMessageCommandId { get; }

    }
}