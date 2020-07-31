using System.Threading;
using NLog;
using Zidium.Storage;

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

        public override int GetMaxDaysFromTariffLimit(TariffLimitForRead tariffLimit)
        {
            return tariffLimit.UnitTestsMaxDays;
        }

        public override EventCategory[] GetCategories()
        {
            return Categories;
        }
    }
}
