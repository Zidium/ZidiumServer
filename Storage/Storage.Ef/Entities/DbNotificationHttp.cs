using System;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Http-часть уведомления, для уведомлений с типом http
    /// </summary>
    public class DbNotificationHttp
    {
        public Guid NotificationId { get; set; }

        public virtual DbNotification Notification { get; set; }

        public string Json { get; set; }
    }
}
