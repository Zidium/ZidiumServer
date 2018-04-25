using System;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Http-часть уведомления, для уведомлений с типом http
    /// </summary>
    public class NotificationHttp
    {
        public Guid NotificationId { get; set; }

        public virtual Notification Notification { get; set; }

        public string Json { get; set; }
    }
}
