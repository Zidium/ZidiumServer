using System.Threading;
using NLog;

namespace Zidium.Agent.AgentTasks.OutdatedEventsStatuses
{
    public class OutdatedEventsStatusesProcessor
    {
        protected ILogger Logger;

        protected CancellationToken CancellationToken;

        public int Count;

        public OutdatedEventsStatusesProcessor(ILogger logger, CancellationToken cancellationToken)
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

                const int maxCount = 100;
                var response = dispatcher.UpdateEventsStatuses(maxCount);
                if (response.Success)
                {
                    var updateCount = response.Data.UpdateCount;
                    Interlocked.Add(ref Count, updateCount);

                    if (updateCount < maxCount)
                    {
                        Logger.Info("Обновлено {0} статусов событий", Count);
                        return;
                    }

                    if (Logger.IsDebugEnabled)
                        Logger.Debug("Обновлено статусов событий: " + updateCount);
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
