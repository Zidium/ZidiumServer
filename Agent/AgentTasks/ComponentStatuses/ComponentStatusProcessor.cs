using System;
using System.Threading;
using NLog;
using Zidium.Core.Common;
using Zidium.Core.ConfigDb;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.ComponentStatuses
{
    public class ComponentStatusProcessor
    {
        protected ILogger Logger;

        public MultipleDataBaseProcessor DbProcessor { get; protected set; }

        public int UpdateStatesCount;

        public ComponentStatusProcessor(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            DbProcessor = new MultipleDataBaseProcessor(logger, cancellationToken);
            DbProcessor.AccountThreads = 1;
            DbProcessor.ComponentsThreads = 1;
            DbProcessor.DataBaseThreads = 1;
        }

        public void Process()
        {
            UpdateStatesCount = 0;
            DbProcessor.ForEachAccount(data =>
            {
                if (data.Account.Type != AccountType.Test)
                    ProcessAccount(data.Account.Id, data.Storage, data.Logger);
            });
            if (UpdateStatesCount > 0)
                Logger.Info("Обновлено статусов: {0}", UpdateStatesCount);
        }

        public void ProcessAccount(Guid accountId, IStorage storage, ILogger logger)
        {
            var dispatcher = AgentHelper.GetDispatcherClient();
            var componentIds = storage.Components.GetAllIds();

            logger.Debug("Найдено компонентов: " + componentIds.Length);
            int index = 0;
            foreach (var componentId in componentIds)
            {
                DbProcessor.CancellationToken.ThrowIfCancellationRequested();

                // пауза, чтобы уменьшить нагрузку на CPU
                index++;
                if (index % 10 == 0)
                {
                    Thread.Sleep(200); 
                }

                var response = dispatcher.UpdateComponentState(accountId, componentId);
                if (response.Success)
                {
                    Interlocked.Increment(ref UpdateStatesCount);
                    logger.Debug("Компонент {0} обновлён успешно.", componentId);
                }
                else
                {
                    DbProcessor.SetException(new Exception(response.ErrorMessage));
                    logger.Error("Компонент {0} обновлён с ошибкой: {1}", componentId, response.ErrorMessage);
                }
            }
        }

    }
}
