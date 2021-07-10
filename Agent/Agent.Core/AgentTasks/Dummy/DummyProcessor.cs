using System.Threading;
using NLog;

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
            Logger.Info("Обработка");
        }

    }
}
