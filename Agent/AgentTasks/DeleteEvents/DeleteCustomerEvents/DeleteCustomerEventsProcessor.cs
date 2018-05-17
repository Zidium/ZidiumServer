using System.Threading;
using NLog;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

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

        public override int GetMaxDaysFromTariffLimit(TariffLimit tariffLimit)
        {
            return tariffLimit.EventsMaxDays;
        }

        public override EventCategory[] GetCategories()
        {
            return Categories;
        }
    }
}
