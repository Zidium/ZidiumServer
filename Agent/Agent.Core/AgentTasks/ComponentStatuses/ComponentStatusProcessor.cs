using System.Threading;
using Microsoft.Extensions.Logging;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.ComponentStatuses
{
    public class ComponentStatusProcessor
    {
        protected ILogger Logger;

        protected CancellationToken CancellationToken;

        public int UpdateStatesCount;

        public ComponentStatusProcessor(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            CancellationToken = cancellationToken;
        }

        public string Process()
        {
            UpdateStatesCount = 0;
            string error = null;

            var dispatcher = AgentHelper.GetDispatcherClient();

            var storage = DependencyInjection.GetServicePersistent<IStorageFactory>().GetStorage();
            var componentIds = storage.Components.GetAllIds();

            Logger.LogDebug("Найдено компонентов: " + componentIds.Length);
            int index = 0;
            foreach (var componentId in componentIds)
            {
                CancellationToken.ThrowIfCancellationRequested();

                // пауза, чтобы уменьшить нагрузку на CPU
                index++;
                if (index % 10 == 0)
                {
                    Thread.Sleep(200);
                }

                var response = dispatcher.UpdateComponentState(componentId);
                if (response.Success)
                {
                    Interlocked.Increment(ref UpdateStatesCount);
                    Logger.LogDebug("Компонент {0} обновлён успешно.", componentId);
                }
                else
                {
                    error = response.ErrorMessage;
                    Logger.LogError("Компонент {0} обновлён с ошибкой: {1}", componentId, response.ErrorMessage);
                }
            }

            if (UpdateStatesCount > 0)
                Logger.LogInformation("Обновлено статусов: {0}", UpdateStatesCount);

            return error;
        }

    }
}
