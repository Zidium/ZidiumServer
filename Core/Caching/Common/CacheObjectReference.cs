using System;

namespace Zidium.Core.Caching
{
    public class CacheObjectReference
    {
        public CacheObjectReference(Guid id, string systemName)
        {
            Id = id;
            SystemName = systemName;
        }

        public Guid Id { get; protected set; }
        
        public string SystemName { get; protected set; }
    }
}
