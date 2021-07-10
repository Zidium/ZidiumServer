using System;

namespace Zidium.Storage
{
    public interface IMetricTypeRepository
    {
        void Add(MetricTypeForAdd entity);

        void Update(MetricTypeForUpdate entity);

        void Update(MetricTypeForUpdate[] entities);

        MetricTypeForRead GetOneById(Guid id);

        MetricTypeForRead GetOneOrNullById(Guid id);

        MetricTypeForRead[] GetMany(Guid[] ids);

        MetricTypeForRead GetOneOrNullBySystemName(string systemName);

        MetricTypeForRead[] Filter(string search, int maxCount);

        int GetCount();

    }
}
