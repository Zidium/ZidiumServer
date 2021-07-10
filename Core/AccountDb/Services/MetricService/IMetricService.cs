using System;
using System.Collections.Generic;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Storage;
using Zidium.Api.Dto;

namespace Zidium.Core.AccountsDb
{
    public interface IMetricService
    {
        IMetricTypeCacheReadObject GetOrCreateType(string name);

        IMetricCacheReadObject CreateMetric(CreateMetricRequestData data);

        Guid CreateMetric(Guid componentId, Guid metricTypeId);

        void DeleteMetric(DeleteMetricRequestData data);

        void DeleteMetricType(Guid metricTypeId);

        void SaveMetrics(List<SendMetricRequestDataDto> data);

        IMetricCacheReadObject SaveMetric(SendMetricRequestDataDto data);

        IMetricCacheReadObject GetActualMetric(Guid componentId, Guid metricTypeId);

        IMetricCacheReadObject GetActualMetric(Guid componentId, string name);

        MetricHistoryForRead[] GetMetricsHistory(GetMetricsHistoryRequestDataDto filter);

        List<IMetricCacheReadObject> GetMetrics(Guid componentId);

        int UpdateMetrics(int maxCount);

        void EnableMetric(Guid metricId);

        void DisableMetric(SetMetricDisableRequestData data);

        void UpdateMetricType(UpdateMetricTypeRequestData data);

        IMetricTypeCacheReadObject CreateMetricType(CreateMetricTypeRequestData data);

        void UpdateMetric(UpdateMetricRequestData data);

        string GetFullDisplayName(MetricForRead metric);
    }
}
