using System;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models
{
    public class BugTrackerEventModel
    {
        public EventType EventType { get; set; }

        public int Count { get; set; }

        public BugTrackerEventsInnerModel[] InnerEvents;
    }

    public class BugTrackerEventsInnerModel
    {
        public Guid Id;
        public Guid ComponentId;
        public DateTime StartDate;
        public string Component;
        public string Version;
    }
}