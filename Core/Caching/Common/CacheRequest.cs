using System;
using Zidium.Core.Caching;

namespace Zidium.Core.Caching
{
    public class CacheRequest : ICacheRequest
    {
        public Guid ObjectId { get; set; }
    }
}
