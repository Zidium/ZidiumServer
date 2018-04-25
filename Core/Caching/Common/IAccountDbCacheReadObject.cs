using System;

namespace Zidium.Core.Caching
{
    public interface IAccountDbCacheReadObject : ICacheReadObject
    {
        Guid AccountId { get; }
    }
}
