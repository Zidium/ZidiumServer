using System;
using System.Collections.Generic;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public interface IMetricService
    {
        IMetricTypeCacheReadObject GetOrCreateType(Guid accountId, string name);

        IMetricCacheReadObject CreateMetric(Guid accountId, CreateMetricRequestData data);

        Guid CreateMetric(Guid accountId, Guid componentId, Guid metricTypeId);

        void DeleteMetric(Guid accountId, DeleteMetricRequestData data);
        
        void DeleteMetricType(Guid accountId, Guid metricTypeId);

        void SaveMetrics(Guid accountId, List<SendMetricRequestData> data);

        IMetricCacheReadObject SaveMetric(Guid accountId, SendMetricRequestData data);

        IMetricCacheReadObject GetActualMetric(Guid accountId, Guid componentId, Guid metricTypeId);

        IMetricCacheReadObject GetActualMetric(Guid accountId, Guid componentId, string name);

        MetricHistoryForRead[] GetMetricsHistory(Guid accountId, GetMetricsHistoryRequestData filter);

        List<IMetricCacheReadObject> GetMetrics(Guid accountId, Guid componentId);

        int UpdateMetrics(Guid accountId, int maxCount);

        void EnableMetric(Guid accountId, Guid metricId);

        void DisableMetric(Guid accountId, SetMetricDisableRequestData data);

        void UpdateMetricType(Guid accountId, UpdateMetricTypeRequestData data);

        IMetricTypeCacheReadObject CreateMetricType(Guid accountId, CreateMetricTypeRequestData data);

        void UpdateMetric(Guid accountId, UpdateMetricRequestData data);

        string GetFullDisplayName(MetricForRead metric);
    }
}
