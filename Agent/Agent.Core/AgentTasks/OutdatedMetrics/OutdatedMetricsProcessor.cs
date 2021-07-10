using System.Threading;
using NLog;

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
                        Logger.Info("Обновлено {0} статусов метрик", Count);
                        return;
                    }

                    if (Logger.IsDebugEnabled)
                        Logger.Debug("Обновлено статусов метрик: " + updateCount);
                }
                else
                {
                    Logger.Error("Ошибка: " + response.ErrorMessage);
                    return;
                }
            }
        }

    }
}
