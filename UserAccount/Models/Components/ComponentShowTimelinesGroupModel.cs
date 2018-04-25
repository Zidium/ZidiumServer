using System;

namespace Zidium.UserAccount.Models
{
    public class ComponentShowTimelinesGroupModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool? HideUptime { get; set; }

        public ComponentShowTimelinesGroupItemModel[] Items { get; set; }
    }
}