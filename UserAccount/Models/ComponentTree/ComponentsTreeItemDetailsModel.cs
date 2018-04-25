using System;

namespace Zidium.UserAccount.Models.ComponentTree
{
    public class ComponentsTreeItemDetailsModel
    {
        public Guid Id { get; set; }

        public ComponentsTreeItemEventsDetailsModel Events { get; set; }

        public ComponentsTreeItemUnittestsDetailsModel Unittests { get; set; }

        public ComponentsTreeItemMetricsDetailsModel Metrics { get; set; }
    }
}