using System;

namespace Zidium.UserAccount.Models.ComponentHistory
{
    public class ComponentsTreeItemUnittestsDetailsItemModel
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public int OkTime { get; set; }

        public TimelineModel Timeline { get; set; }
    }
}