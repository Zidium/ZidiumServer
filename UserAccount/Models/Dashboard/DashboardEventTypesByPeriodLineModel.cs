using System.Linq;

namespace Zidium.UserAccount.Models
{
    public class DashboardEventTypesByPeriodLineModel
    {
        public DashboardEventTypesByPeriodLineModel(DashboardEventModel[] events)
        {
            TotalCount = events.GroupBy(t => t.EventTypeId).Count();
            ProcessedCount = events.Where(t => t.IsProcessed).GroupBy(t => t.EventTypeId).Count();
            NotProcessedCount = TotalCount - ProcessedCount;
        }

        public int TotalCount { get; set; }
        public int ProcessedCount { get; set; }
        public int NotProcessedCount { get; set; }
    }
}