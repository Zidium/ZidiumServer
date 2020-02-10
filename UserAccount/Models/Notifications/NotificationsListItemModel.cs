using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models
{
    public class NotificationsListItemModel
    {
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }

        public Event Event { get; set; }

        public Component Component { get; set; }

        public string Address { get; set; }

        public User User { get; set; }

        public SubscriptionChannel Channel { get; set; }

        public NotificationStatus Status { get; set; }

        public string SendError { get; set; }

        public DateTime? SendDate { get; set; }

        public DateTime? NextDate { get; set; }

        public EventType EventType { get; set; }
    }
}