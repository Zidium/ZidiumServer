using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Http-часть уведомления, для уведомлений с типом http
    /// </summary>
    public class NotificationHttpForUpdate
    {
        public NotificationHttpForUpdate(Guid notificationId)
        {
            NotificationId = notificationId;
            Json = new ChangeTracker<string>();
        }

        public Guid NotificationId { get; }

        public ChangeTracker<string> Json { get; }
    }
}