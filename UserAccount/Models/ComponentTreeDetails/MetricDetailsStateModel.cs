using System;
using Zidium.Api.Dto;

namespace Zidium.UserAccount.Models.ComponentTreeDetails
{
    public class MetricDetailsStateModel
    {
        public Guid Id { get; set; }

        public MonitoringStatus Status { get; set; }

        public Guid StatusEventId { get; set; }

        public TimeSpan StatusDuration { get; set; }

        public DateTime LastResultDate { get; set; }

        public double? Value { get; set; }

        public DateTime ActualDate { get; set; }

        public TimeSpan ActualInterval { get; set; }

        public bool IsEnabled { get; set; }

        public bool HasSignal { get; set; }

        public bool CanEdit { get; set; }

        public string ValueText
        {
            get
            {
                if (!HasSignal)
                    return "Нет значения";

                if (Value.HasValue)
                    return Value.ToString();

                return "null";
            }
        }
    }
}