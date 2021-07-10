using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.MetricData
{
    public class MetricDataListModel
    {
        public Guid? ComponentId { get; set; }

        public Guid? MetricTypeId { get; set; }

        public string CounterName { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public MetricHistoryForRead[] Data { get; set; }

        public MetricGraphDataModel Graph { get; set; }

    }
}