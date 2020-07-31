using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class DashboardComponentsInfoModel
    {
        public ComponentTypeForRead ComponentType { get; set; }
        public int TotalCount { get; set; }
        public int AlarmCount { get; set; }
        public int WarningCount { get; set; }
        public int SuccessCount { get; set; }
        public int OtherCount { get; set; }
    }
}