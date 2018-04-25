using System;
using Zidium.Core.AccountsDb;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models
{
    public class SubscriptionEditModel
    {
        public Guid? Id { get; set; }
        
        public Guid? UserId { get; set; }
        
        public Guid? ComponentTypeId { get; set; }

        public Guid? ComponentId { get; set; }

        public SubscriptionObject Object { get; set; }
        
        public SubscriptionChannel? Channel { get; set; }

        public bool CanShowChannel { get; set; }

        public bool CanShowComponentType { get; set; }

        public bool CanShowComponent { get; set; }

        public bool IsEnabled { get; set; }
        
        public ColorStatusSelectorValue Color { get; set; }
        
        public TimeSpan? MinimumDuration { get; set; }
        
        public TimeSpan? ResendTime { get; set; }

        public Exception Exception { get; set; }

        public bool NotifyBetterStatus { get; set; }

        public string ReturnUrl { get; set; }

        public bool ModalMode { get; set; }

        public bool IsNew => Id == null;

        public bool IsExists => Id.HasValue;
    }
}