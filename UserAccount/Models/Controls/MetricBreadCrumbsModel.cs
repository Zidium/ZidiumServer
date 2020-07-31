using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class MetricBreadCrumbsModel
    {
        public ComponentBreadCrumbsModel ComponentBreadCrumbs;

        public string DisplayName;

        public Guid Id;

        public static MetricBreadCrumbsModel Create(Guid id, IStorage storage)
        {
            var metric = storage.Metrics.GetOneById(id);
            var metricType = storage.MetricTypes.GetOneById(metric.MetricTypeId);
            return new MetricBreadCrumbsModel()
            {
                Id = id,
                DisplayName = metricType.DisplayName,
                ComponentBreadCrumbs = ComponentBreadCrumbsModel.Create(metric.ComponentId, storage)
            };
        }
    }
}