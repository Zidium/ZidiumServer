using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class NotificationsListItemModel
    {
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }

        public EventForRead Event { get; set; }

        public ComponentForRead Component { get; set; }

        public string Address { get; set; }

        public UserForRead User { get; set; }

        public SubscriptionChannel Channel { get; set; }

        public NotificationStatus Status { get; set; }

        public string SendError { get; set; }

        public DateTime? SendDate { get; set; }

        public DateTime? NextDate { get; set; }

        public EventTypeForRead EventType { get; set; }
    }
}