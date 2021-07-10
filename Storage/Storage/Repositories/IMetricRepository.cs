using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public interface IMetricRepository
    {
        void Add(MetricForAdd entity);

        void Update(MetricForUpdate[] entities);

        MetricForRead GetOneById(Guid id);

        MetricForRead GetOneOrNullById(Guid id);

        Guid[] GetNotActualIds(int maxCount, DateTime now);

        Guid[] GetByMetricTypeId(Guid metricTypeId);

        MetricForRead[] GetByComponentId(Guid componentId);

        /*
         * query.Where(t => statuses.Contains(t.Bulb.Status))
         * OrderBy(t => t.Component.DisplayName).ThenBy(t => t.MetricType.DisplayName)
         * TOP 100
         */
        MetricForRead[] Filter(Guid? metricTypeId, Guid? componentId, MonitoringStatus[] statuses, int maxCount);

    }
}
