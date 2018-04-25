using System.Linq;
using Zidium.Core.Api;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models
{
    public class DashboardComponentsInfoModel
    {
        public DashboardComponentsInfoModel(Component[] components, ComponentType componentType = null)
        {
            ComponentType = componentType; 
            TotalCount = components.Count();
            AlarmCount = components.Count(t => t.ExternalStatus.Status == MonitoringStatus.Alarm);
            WarningCount = components.Count(t => t.ExternalStatus.Status == MonitoringStatus.Warning);
            SuccessCount = components.Count(t => t.ExternalStatus.Status == MonitoringStatus.Success);
            OtherCount = components.Count(t => t.ExternalStatus.Status == MonitoringStatus.Disabled || t.ExternalStatus.Status == MonitoringStatus.Unknown);
        }

        public ComponentType ComponentType { get; set; }
        public int TotalCount { get; set; }
        public int AlarmCount { get; set; }
        public int WarningCount { get; set; }
        public int SuccessCount { get; set; }
        public int OtherCount { get; set; }
    }
}