using System;

namespace Zidium.UserAccount.Models
{
    public class ComponentTypeSelectorOptions
    {
        public Guid? SelectedValue { get; set; }

        public bool AllowEmpty { get; set; }

        public bool AutoRefreshPage { get; set; }

        public bool HideWhenFilter { get; set; }

        public ComponentTypeSelectorMode Mode { get; set; }
    }
}