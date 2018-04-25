using System.Linq;
using System.Threading;
using NLog;
using Zidium.Api;
using Zidium.Core.AccountsDb;

namespace Zidium.Agent.AgentTasks.DeleteEvents
{
    public class DeleteComponentEventsProcessor : DeleteEventsProcessorBase
    {
        public DeleteComponentEventsProcessor(ILogger logger, CancellationToken cancellationToken)
            : base(logger, cancellationToken)
        {
            var categories = new[]
            {
                EventCategory.ComponentExternalStatus,
                EventCategory.ComponentInternalStatus,
                EventCategory.ComponentChildsStatus
            };

            Categories = string.Join(", ", categories.Select(t => (int)t));
        }

        protected string Categories;

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

        public override string GetCategories()
        {
            return Categories;
        }
    }
}
