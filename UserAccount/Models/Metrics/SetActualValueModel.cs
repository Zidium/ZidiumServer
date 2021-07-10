using System;

namespace Zidium.UserAccount.Models.Metrics
{
    public class SetActualValueModel
    {
        public Guid Id { get; set; }

        public string Value { get; set; }

        public TimeSpan? ActualTime { get; set; }

        public string MetricTypeName { get; set; }
    }
}