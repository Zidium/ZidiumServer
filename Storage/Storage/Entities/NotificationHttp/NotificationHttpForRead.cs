using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Http-часть уведомления, для уведомлений с типом http
    /// </summary>
    public class NotificationHttpForRead
    {
        public NotificationHttpForRead(Guid notificationId, string json)
        {
            NotificationId = notificationId;
            Json = json;
        }

        public Guid NotificationId { get; }

        public string Json { get; }

        public NotificationHttpForUpdate GetForUpdate()
        {
            return new NotificationHttpForUpdate(NotificationId);
        }

    }
}
