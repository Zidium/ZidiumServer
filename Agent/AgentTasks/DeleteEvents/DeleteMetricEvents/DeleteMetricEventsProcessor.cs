using System.Threading;
using NLog;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.DeleteEvents
{
    public class DeleteMetricEventsProcessor : DeleteEventsProcessorBase
    {
        public DeleteMetricEventsProcessor(ILogger logger, CancellationToken cancellationToken)
            : base(logger, cancellationToken)
        {
            Categories = new[]
            {
                EventCategory.MetricStatus,
                EventCategory.ComponentMetricsStatus
            };
        }

        protected EventCategory[] Categories;

        public override int GetMaxDaysFromTariffLimit(TariffLimitForRead tariffLimit)
        {
            return tariffLimit.MetricsMaxDays;
        }

        public override EventCategory[] GetCategories()
        {
            return Categories;
        }
    }
}
