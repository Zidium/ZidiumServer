using System;
using Zidium.Core.AccountsDb;

namespace Zidium.Core.Api
{
    public class GetOrCreateSubscriptionRequestData
    {
        public Guid UserId { get; set; }

        public Guid? ComponentTypeId { get; set; }

        public SubscriptionChannel Channel { get; set; }
    }
}
