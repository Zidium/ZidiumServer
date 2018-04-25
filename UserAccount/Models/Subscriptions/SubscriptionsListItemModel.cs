using System;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models
{
    public class SubscriptionsListItemModel
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid? ComponentTypeId { get; set; }

        public string ComponentTypeName { get; set; }

        public int? ComponentsCount { get; set; }

        public bool IsEnabled { get; set; }

        public EventImportance Importance { get; set; }

        public bool UseInternalStatus { get; set; }

        public TimeSpan? MinDuration { get; set; }

        public TimeSpan? ResendInterval { get; set; }
    }
}