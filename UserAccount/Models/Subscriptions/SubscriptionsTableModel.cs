using System;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models.Subscriptions
{
    public class SubscriptionsTableModel
    {
        public Guid UserId { get; set; }

        public SubscriptionObject Object { get; set; }

        public string SubscriptionName
        {
            get
            {
                if (Object == SubscriptionObject.Default)
                {
                    return "Название";
                }
                if (Object == SubscriptionObject.ComponentType)
                {
                    return "Тип компонента";
                }
                if (Object == SubscriptionObject.Component)
                {
                    return "Компонент";
                }
                return "";
            }
        }

        public SubscriptionsTableRowModel[] Rows { get; set; }
    }
}