﻿using System;
using Zidium.Api.Dto;
using Zidium.Storage;

namespace Zidium.Core.Api
{
    public class SubscriptionInfo
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid? ComponentTypeId { get; set; }

        public bool IsEnabled { get; set; }

        public bool NotifyBetterStatus { get; set; }

        public EventImportance Importance { get; set; }

        public int? DurationMinimumInSeconds { get; set; }

        public int? ResendTimeInSeconds { get; set; }

        public SubscriptionChannel Channel { get; set; }

        public bool SendOnlyInInterval { get; set; }

        public int? SendIntervalFromHour { get; set; }

        public int? SendIntervalFromMinute { get; set; }

        public int? SendIntervalToHour { get; set; }

        public int? SendIntervalToMinute { get; set; }

    }
}
