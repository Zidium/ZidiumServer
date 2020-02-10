using System;

namespace Zidium.Core.Api
{
    public class UpdateSubscriptionRequestData
    {
        public Guid Id { get; set; }

        public EventImportance Importance { get; set; }

        public bool IsEnabled { get; set; }

        public bool NotifyBetterStatus { get; set; }
        
        public int? DurationMinimumInSeconds { get; set; }

        public int? ResendTimeInSeconds { get; set; }

        public bool SendOnlyInInterval { get; set; }

        public int? SendIntervalFromHour { get; set; }

        public int? SendIntervalFromMinute { get; set; }

        public int? SendIntervalToHour { get; set; }

        public int? SendIntervalToMinute { get; set; }

    }
}
