using System;

namespace Zidium.Storage
{
    public interface IMetricHistoryRepository
    {
        void Add(MetricHistoryForAdd entity);

        MetricHistoryForRead[] GetByPeriod(
            Guid componentId, 
            DateTime? startDate, 
            DateTime? endDate, 
            Guid[] metricTypes,
            int maxCount
            );

        MetricHistoryForRead GetFirstByStatusEventId(Guid statusEventId);

        int DeleteMetricsHistory(int maxCount, DateTime toDate);

        MetricHistoryForRead[] GetLast(Guid componentId, Guid metricTypeId, int maxCount);

    }
}
