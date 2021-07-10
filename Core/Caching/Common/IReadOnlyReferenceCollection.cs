using System;

namespace Zidium.Core.Caching
{
    public interface IReadOnlyReferenceCollection
    {
        int Count { get; }

        CacheObjectReference FindByName(string name);

        CacheObjectReference FindByMetricTypeId(Guid typeId);

        CacheObjectReference[] GetAll();
    }
}
