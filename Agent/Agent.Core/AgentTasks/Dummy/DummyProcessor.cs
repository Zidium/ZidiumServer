using System.Threading;
using Microsoft.Extensions.Logging;

namespace Zidium.Agent.AgentTasks.Dummy
{
    public class DummyProcessor
    {
        protected ILogger Logger;

        public DummyProcessor(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
        }

        public void Process()
        {
            Logger.LogInformation("Обработка");
        }

    }
}
