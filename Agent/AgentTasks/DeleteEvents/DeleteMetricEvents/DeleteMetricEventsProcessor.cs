using System.Threading;
using NLog;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

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

        public override int GetMaxDaysFromTariffLimit(TariffLimit tariffLimit)
        {
            return tariffLimit.MetricsMaxDays;
        }

        public override EventCategory[] GetCategories()
        {
            return Categories;
        }
    }
}
