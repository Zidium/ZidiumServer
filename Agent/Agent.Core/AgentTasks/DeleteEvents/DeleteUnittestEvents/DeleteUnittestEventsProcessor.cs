using System.Threading;
using NLog;
using Zidium.Api.Dto;

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

        public override int GetMaxDays()
        {
            var logicSettings = LogicSettingsCache.LogicSettings;
            return logicSettings.UnitTestsMaxDays;
        }

        public override EventCategory[] GetCategories()
        {
            return Categories;
        }
    }
}
