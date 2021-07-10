using System.Linq;
using System.Threading;
using NLog;
using Zidium.Api.Dto;

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

        public override EventCategory[] GetCategories()
        {
            return Categories;
        }

        public override int GetMaxDays()
        {
            var logicSettings = LogicSettingsCache.LogicSettings;
            var maxDays = new[]
            {
                logicSettings.EventsMaxDays,
                logicSettings.UnitTestsMaxDays,
                logicSettings.MetricsMaxDays
            };
            return maxDays.Max();
        }
    }
}
