using System.Linq;

namespace Zidium.UserAccount.Models
{
    public class DashboardEventsByPeriodLineModel
    {
        public DashboardEventsByPeriodLineModel(DashboardEventModel[] events)
        {
            TotalCount = events.Count();
            ProcessedCount = events.Count(t => t.IsProcessed);
            NotProcessedCount = TotalCount - ProcessedCount;
        }

        public int TotalCount { get; set; }
        public int ProcessedCount { get; set; }
        public int NotProcessedCount { get; set; }
    }
}