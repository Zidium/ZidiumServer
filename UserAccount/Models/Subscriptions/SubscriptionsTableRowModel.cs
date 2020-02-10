using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models.Subscriptions
{
    public class SubscriptionsTableRowModel
    {
        public List<ShowSubscriptionCellModel> Cells { get; set; }

        public SubscriptionsTableModel Table { get; set; }

        public Subscription GetFirstNotNull()
        {
            var result = Cells.FirstOrDefault(t => t.Subscription != null);

            if (result != null)
                return result.Subscription;

            throw new Exception("GetFirstNotNull не удалось найти");
        }
    }
}