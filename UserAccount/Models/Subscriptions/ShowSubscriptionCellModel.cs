using System;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models.Subscriptions
{
    public class ShowSubscriptionCellModel
    {
        public Subscription Subscription { get; set; }

        public SubscriptionObject Object { get; set; }

        public SubscriptionChannel Channel { get; set; }

        public Guid? ObjectId { get; set; }

        public SubscriptionsTableRowModel Row { get; set; }
    }
}