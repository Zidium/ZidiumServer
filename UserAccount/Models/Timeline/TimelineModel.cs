using System;
using System.Collections.Generic;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models
{
    public class TimelineModel
    {
        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public List<TimelineItemModel> Items { get; set; }

        public int OkTime { get; set; }

        public EventCategory? Category { get; set; }

        public Guid? OwnerId { get; set; }

        public Guid? EventTypeId { get; set; }

        public bool HideUptime { get; set; }
    }
}