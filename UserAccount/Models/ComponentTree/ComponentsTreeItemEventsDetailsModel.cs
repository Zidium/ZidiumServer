using System;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models.ComponentTree
{
    public class ComponentsTreeItemEventsDetailsModel
    {
        public bool HasEvents { get; set; }

        public MonitoringStatus Status { get; set; }

        public DateTime FromDate { get; set; }

        public TimeSpan StatusDuration { get; set; }
    }
}