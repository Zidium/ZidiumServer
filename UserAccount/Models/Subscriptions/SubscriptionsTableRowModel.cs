using System;
using System.Collections.Generic;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.Subscriptions
{
    public class SubscriptionsTableRowModel
    {
        public List<ShowSubscriptionCellModel> Cells { get; set; }

        public SubscriptionsTableModel Table { get; set; }

        public SubscriptionObject SubscriptionObject { get; set; }

        public Guid? ComponentId { get; set; }

        public string Text { get; set; }

    }
}