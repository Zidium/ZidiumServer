using System.Threading;
using NLog;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.Agent.AgentTasks.DeleteEvents
{
    public class DeleteUnittestEventsProcessor : DeleteEventsProcessorBase
    {
        public DeleteUnittestEventsProcessor(ILogger logger, CancellationToken cancellationToken)
            : base(logger, cancellationToken)
        {
            Categories = new[]
            {
                EventCategory.UnitTestStatus,
                EventCategory.UnitTestResult,
                EventCategory.ComponentUnitTestsStatus
            };
        }

        protected EventCategory[] Categories;

        public override int GetMaxDaysFromTariffLimit(TariffLimit tariffLimit)
        {
            return tariffLimit.UnitTestsMaxDays;
        }

        public override EventCategory[] GetCategories()
        {
            return Categories;
        }
    }
}
