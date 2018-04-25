using System;
using Zidium.Core.AccountsDb;

namespace Zidium.Core.Api
{
    public class CreateUserDefaultSubscriptionRequestData
    {
        public Guid UserId { get; set; }

        public SubscriptionChannel Channel { get; set; }
    }
}
