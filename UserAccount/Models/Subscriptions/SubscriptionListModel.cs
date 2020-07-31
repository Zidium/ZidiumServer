using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.Subscriptions
{
    public class SubscriptionListModel
    {
        public Guid UserId { get; set; }

        public string Color { get; set; }

        public SubscriptionForRead[] Subscriptions { get; set; }

        public SubscriptionChannel[] Channels { get; set; }

        public SubscriptionsTableModel DefaultSubscriptions;

        public SubscriptionsTableModel ComponentTypeSubscriptions;

        public SubscriptionsTableModel ComponentSubscriptions;

    }
}