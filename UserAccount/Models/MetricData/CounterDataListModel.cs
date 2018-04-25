using System;
using System.Collections.Generic;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models.MetricData
{
    public class CounterDataListModel
    {
        public Guid? ComponentId { get; set; }

        public Guid? CounterId { get; set; }

        public string CounterName { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public MetricHistory[] Data { get; set; }

        public MetricGraphDataModel Graph { get; set; }
    }
}