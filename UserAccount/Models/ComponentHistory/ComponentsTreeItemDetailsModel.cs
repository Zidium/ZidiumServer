using System;

namespace Zidium.UserAccount.Models.ComponentHistory
{
    public class ComponentsTreeItemDetailsModel
    {
        public Guid Id { get; set; }

        public bool HasEvents { get; set; }

        public ComponentsTreeItemUnittestsDetailsModel Unittests { get; set; }

        public ComponentsTreeItemMetricsDetailsModel Metrics { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public string Path { get; set; }

        public int EventsOkTime { get; set; }

        public TimelineModel EventsTimeline { get; set; }

        public int UnittestsOkTime { get; set; }

        public TimelineModel UnittestsTimeline { get; set; }

        public int MetricsOkTime { get; set; }

        public TimelineModel MetricsTimeline { get; set; }
    }
}