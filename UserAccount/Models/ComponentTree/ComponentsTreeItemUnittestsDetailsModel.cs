using System;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models.ComponentTree
{
    public class ComponentsTreeItemUnittestsDetailsModel
    {
        public MonitoringStatus Status { get; set; }

        public ComponentsTreeItemUnittestsDetailsItemModel[] Items { get; set; }

        public TimeSpan StatusDuration { get; set; }

        public bool Expanded { get; set; }

        public Guid Id { get; set; }

        public string HtmlId
        {
            get
            {
                return "unittests-" + Id;
            }
        }
    }
}