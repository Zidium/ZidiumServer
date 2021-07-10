using System.Threading;
using Microsoft.Extensions.Logging;
using Zidium.Api.Dto;

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

        public override int GetMaxDays()
        {
            var logicSettings = LogicSettingsCache.LogicSettings;
            return logicSettings.EventsMaxDays;
        }

        public override EventCategory[] GetCategories()
        {
            return Categories;
        }
    }
}
