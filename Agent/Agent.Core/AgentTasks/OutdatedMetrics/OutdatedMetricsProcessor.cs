using System.Threading;
using Microsoft.Extensions.Logging;

namespace Zidium.Agent.AgentTasks.OutdatedMetrics
{
    public class OutdatedMetricsProcessor
    {
        protected ILogger Logger;

        protected CancellationToken CancellationToken;

        public int Count;

        public int MaxUpdateCount = 100;

        public OutdatedMetricsProcessor(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            CancellationToken = cancellationToken;
        }

        public void Process()
        {
            Count = 0;

            var dispatcher = AgentHelper.GetDispatcherClient();
            while (true)
            {
                CancellationToken.ThrowIfCancellationRequested();

                var response = dispatcher.CalculateMetrics(MaxUpdateCount);
                if (response.Success)
                {
                    var updateCount = response.Data.UpdateCount;
                    Interlocked.Add(ref Count, updateCount);
                    if (updateCount < MaxUpdateCount)
                    {
                        Logger.LogInformation("Обновлено {0} статусов метрик", Count);
                        return;
                    }

                    Logger.LogDebug("Обновлено статусов метрик: " + updateCount);
                }
                else
                {
                    Logger.LogError("Ошибка: " + response.ErrorMessage);
                    return;
                }
            }
        }

    }
}
