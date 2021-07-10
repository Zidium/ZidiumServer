using System;
using Zidium.Api.Dto;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class NotificationsListModel
    {
        public Guid? ComponentId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public EventCategory? Category { get; set; }

        public SubscriptionChannel? Channel { get; set; }

        public NotificationStatus? Status { get; set; }

        public Guid? UserId { get; set; }

        public NotificationsListItemModel[] Notifications { get; set; }

        public const int MaxMessageLength = 100;
    }
}