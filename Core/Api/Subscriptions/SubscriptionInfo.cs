using System;
using System.Runtime.Serialization;

namespace Zidium.Core.Api
{
    [DataContract]
    public class SubscriptionInfo
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public Guid? ComponentTypeId { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public bool NotifyBetterStatus { get; set; }

        [DataMember]
        public EventImportance Importance { get; set; }
        
        [DataMember]
        public int? DurationMinimumInSeconds { get; set; }

        [DataMember]
        public int? ResendTimeInSeconds { get; set; }

        [DataMember]
        public SubscriptionChannel Channel { get; set; }

        [DataMember]
        public bool SendOnlyInInterval { get; set; }

        [DataMember]
        public int? SendIntervalFromHour { get; set; }

        [DataMember]
        public int? SendIntervalFromMinute { get; set; }

        [DataMember]
        public int? SendIntervalToHour { get; set; }

        [DataMember]
        public int? SendIntervalToMinute { get; set; }

    }
}
