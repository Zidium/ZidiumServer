using System;
using System.Collections.Generic;
using Zidium.Storage;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models
{
    public class SubscriptionsListModel
    {
        public Guid UserId { get; set; }

        public SubscriptionChannel Channel { get; set; }

        public SubscriptionChannel[] Channels { get; set; }

        public List<SubscriptionsListItemModel> Items { get; set; }

        public ColorStatusSelectorValue DummyColor;
    }
}