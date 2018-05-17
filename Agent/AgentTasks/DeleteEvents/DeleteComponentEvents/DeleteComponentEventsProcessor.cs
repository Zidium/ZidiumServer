using System.Linq;
using System.Threading;
using NLog;
using Zidium.Core.AccountsDb;

namespace Zidium.Agent.AgentTasks.DeleteEvents
{
    public class DeleteComponentEventsProcessor : DeleteEventsProcessorBase
    {
        public DeleteComponentEventsProcessor(ILogger logger, CancellationToken cancellationToken)
            : base(logger, cancellationToken)
        {
            Categories = new[]
            {
                Core.Api.EventCategory.ComponentExternalStatus,
                Core.Api.EventCategory.ComponentInternalStatus,
                Core.Api.EventCategory.ComponentChildsStatus
            };
        }

        protected Core.Api.EventCategory[] Categories;

        public override int GetMaxDaysFromTariffLimit(TariffLimit tariffLimit)
        {
            var maxDays = new[]
            {
                tariffLimit.EventsMaxDays,
                tariffLimit.UnitTestsMaxDays,
                tariffLimit.MetricsMaxDays
            };
            return maxDays.Max();
        }

        public override Core.Api.EventCategory[] GetCategories()
        {
            return Categories;
        }
    }
}
