using System.Linq;
using System.Threading;
using NLog;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.DeleteEvents
{
    public class DeleteComponentEventsProcessor : DeleteEventsProcessorBase
    {
        public DeleteComponentEventsProcessor(ILogger logger, CancellationToken cancellationToken)
            : base(logger, cancellationToken)
        {
            Categories = new[]
            {
                EventCategory.ComponentExternalStatus,
                EventCategory.ComponentInternalStatus,
                EventCategory.ComponentChildsStatus
            };
        }

        protected EventCategory[] Categories;

        public override int GetMaxDaysFromTariffLimit(TariffLimitForRead tariffLimit)
        {
            var maxDays = new[]
            {
                tariffLimit.EventsMaxDays,
                tariffLimit.UnitTestsMaxDays,
                tariffLimit.MetricsMaxDays
            };
            return maxDays.Max();
        }

        public override EventCategory[] GetCategories()
        {
            return Categories;
        }
    }
}
