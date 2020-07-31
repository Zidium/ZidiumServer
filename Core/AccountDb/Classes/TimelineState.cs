using System;
using Zidium.Storage;

namespace Zidium.Core.AccountDb
{
    public class TimelineState
    {
        public DateTime EventStartDate { get; set; }

        public DateTime EventEndDate { get; set; }

        public DateTime EventActualDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public EventImportance Status { get; set; }

        public string Message { get; set; }

        public Guid Id { get; set; }

        public EventCategory Category { get; set; }

        public Guid OwnerId { get; set; }

        public int Count { get; set; }
    }
}
