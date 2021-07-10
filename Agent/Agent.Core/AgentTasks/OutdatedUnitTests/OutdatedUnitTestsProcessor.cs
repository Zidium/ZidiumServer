using System.Threading;
using Microsoft.Extensions.Logging;

namespace Zidium.Agent.AgentTasks.OutdatedUnitTests
{
    /// <summary>
    /// Обновляет устаревшие статусы проверок
    /// </summary>
    public class OutdatedUnitTestsProcessor
    {
        protected ILogger Logger;

        protected CancellationToken CancellationToken;

        public int Count;

        public OutdatedUnitTestsProcessor(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            CancellationToken = cancellationToken;
        }

        public void Process()
        {
            var dispatcher = AgentHelper.GetDispatcherClient();
            while (true)
            {
                CancellationToken.ThrowIfCancellationRequested();

                const int maxCount = 100;
                var response = dispatcher.RecalcUnitTestsResults(maxCount);
                if (response.Success)
                {
                    var updateCount = response.Data.UpdateCount;
                    Interlocked.Add(ref Count, updateCount);
                    if (updateCount < maxCount)
                    {
                        Logger.LogInformation("Обновлено {0} статусов проверок", Count);
                        return;
                    }

                    Logger.LogDebug("Обновлено статусов проверок: " + updateCount);
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
