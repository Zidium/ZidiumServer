using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class ComponentStatusSelectorModel : SelectorModel
    {
        public ComponentStatusSelectorModel(string name, MonitoringStatus[] statuses, bool autoRefreshPage, bool hideWhenFilter = false)
        {
            Name = name;
            Statuses = statuses;
            AutoRefreshPage = autoRefreshPage;
            HideWhenFilter = hideWhenFilter;
        }

        public MonitoringStatus[] Statuses { get; set; }
    }
}