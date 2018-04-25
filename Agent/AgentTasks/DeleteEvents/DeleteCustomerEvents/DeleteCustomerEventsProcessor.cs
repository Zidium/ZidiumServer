using System.Linq;
using System.Threading;
using NLog;
using Zidium.Api;
using Zidium.Core.AccountsDb;

namespace Zidium.Agent.AgentTasks.DeleteEvents
{
    public class DeleteCustomerEventsProcessor : DeleteEventsProcessorBase
    {
        public DeleteCustomerEventsProcessor(ILogger logger, CancellationToken cancellationToken)
            : base(logger, cancellationToken)
        {
            var categories = new[]
            {
                EventCategory.ComponentEvent,
                EventCategory.ApplicationError,
                EventCategory.ComponentEventsStatus
            };

            Categories = string.Join(", ", categories.Select(t => (int)t));
        }

        protected string Categories;

        public override int GetMaxDaysFromTariffLimit(TariffLimit tariffLimit)
        {
            return tariffLimit.EventsMaxDays;
        }

        public override string GetCategories()
        {
            return Categories;
        }
    }
}
