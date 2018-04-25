using System;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models.Subscriptions
{
    public class SubscriptionsTableRowModel
    {
        public ShowSubscriptionCellModel Email { get; set; }

        public ShowSubscriptionCellModel Sms { get; set; }

        public SubscriptionsTableModel Table { get; set; }

        public Subscription GetFirstNotNull()
        {
            if (Email.Subscription != null)
            {
                return Email.Subscription;
            }
            if (Sms.Subscription != null)
            {
                return Sms.Subscription;
            }
            throw new Exception("GetFirstNotNull не удалось найти");
        }
    }
}