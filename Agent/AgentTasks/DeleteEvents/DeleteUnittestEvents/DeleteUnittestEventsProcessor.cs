using System.Linq;
using System.Threading;
using NLog;
using Zidium.Api;
using Zidium.Core.AccountsDb;

namespace Zidium.Agent.AgentTasks.DeleteEvents
{
    public class DeleteUnittestEventsProcessor : DeleteEventsProcessorBase
    {
        public DeleteUnittestEventsProcessor(ILogger logger, CancellationToken cancellationToken)
            : base(logger, cancellationToken)
        {
            var categories = new[]
            {
                EventCategory.UnitTestStatus,
                EventCategory.UnitTestResult,
                EventCategory.ComponentUnitTestsStatus
            };

            Categories = string.Join(", ", categories.Select(t => (int)t));
        }

        protected string Categories;

        public override int GetMaxDaysFromTariffLimit(TariffLimit tariffLimit)
        {
            return tariffLimit.UnitTestsMaxDays;
        }

        public override string GetCategories()
        {
            return Categories;
        }
    }
}
