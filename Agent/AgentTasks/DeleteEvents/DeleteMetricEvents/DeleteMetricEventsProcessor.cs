using System.Linq;
using System.Threading;
using NLog;
using Zidium.Api;
using Zidium.Core.AccountsDb;

namespace Zidium.Agent.AgentTasks.DeleteEvents
{
    public class DeleteMetricEventsProcessor : DeleteEventsProcessorBase
    {
        public DeleteMetricEventsProcessor(ILogger logger, CancellationToken cancellationToken)
            : base(logger, cancellationToken)
        {
            var categories = new[]
            {
                EventCategory.MetricStatus,
                EventCategory.ComponentMetricsStatus
            };

            Categories = string.Join(", ", categories.Select(t => (int)t));
        }

        protected string Categories;

        public override int GetMaxDaysFromTariffLimit(TariffLimit tariffLimit)
        {
            return tariffLimit.MetricsMaxDays;
        }

        public override string GetCategories()
        {
            return Categories;
        }
    }
}
