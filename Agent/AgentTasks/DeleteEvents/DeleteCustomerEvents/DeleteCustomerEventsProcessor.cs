using System.Threading;
using NLog;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.DeleteEvents
{
    public class DeleteCustomerEventsProcessor : DeleteEventsProcessorBase
    {
        public DeleteCustomerEventsProcessor(ILogger logger, CancellationToken cancellationToken)
            : base(logger, cancellationToken)
        {
            Categories = new[]
            {
                EventCategory.ComponentEvent,
                EventCategory.ApplicationError,
                EventCategory.ComponentEventsStatus
            };
        }

        protected EventCategory[] Categories;

        public override int GetMaxDaysFromTariffLimit(TariffLimitForRead tariffLimit)
        {
            return tariffLimit.EventsMaxDays;
        }

        public override EventCategory[] GetCategories()
        {
            return Categories;
        }
    }
}
