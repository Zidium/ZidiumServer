using System;

namespace Zidium.UserAccount.Models.ComponentHistory
{
    public class ComponentsTreeItemMetricsDetailsModel
    {
        public ComponentsTreeItemMetricsDetailsItemModel[] Items { get; set; }

        public bool Expanded { get; set; }

        public Guid Id { get; set; }

        public string HtmlId
        {
            get
            {
                return "metrics-" + Id;
            }
        }

    }
}