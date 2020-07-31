using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.MetricData
{
    public class MetricGraphDataItem
    {
        public DateTime Date { get; set; }

        public double? Value { get; set; }

        public ObjectColor Color { get; set; }
    }
}