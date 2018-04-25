using System;

namespace Zidium.Core.Caching
{
    public class AccountCacheRequest : ICacheRequest
    {
        public Guid ObjectId { get; set; }

        public Guid AccountId { get; set; }
    }
}
