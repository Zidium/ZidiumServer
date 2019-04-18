using System;
using System.Linq;
using System.Threading;
using NLog;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.ConfigDb;

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
        }

        public void Process()
        {
            UpdateStatesCount = 0;
            DbProcessor.ForEachAccount(data =>
            {
                if (data.Account.Type != AccountType.Test)
                    ProcessAccount(data.Account.Id, data.AccountDbContext, data.Logger);
            });
            if (UpdateStatesCount > 0)
                Logger.Info("Обновлено статусов: {0}", UpdateStatesCount);
        }

        public void ProcessAccount(Guid accountId, AccountDbContext accountDbContext, ILogger logger)
        {
            var dispatcher = AgentHelper.GetDispatcherClient();
            var repository = accountDbContext.GetComponentRepository();
            var components = repository.QueryAll()
                .Select(t => new { t.Id, t.SystemName })
                .ToArray();

            logger.Debug("Найдено компонентов: " + components.Length);
            foreach (var component in components)
            {
                DbProcessor.CancellationToken.ThrowIfCancellationRequested();

                var response = dispatcher.UpdateComponentState(accountId, component.Id);
                if (response.Success)
                {
                    Interlocked.Increment(ref UpdateStatesCount);
                    logger.Debug("ComponentId: {0} name {1}; Обновлен успешно.", component.Id, component.SystemName);
                }
                else
                {
                    DbProcessor.SetException(new Exception(response.ErrorMessage));
                    logger.Error("ComponentId: {0} name {1}; Ошибка проверки: {2}", component.Id, component.SystemName, response.ErrorMessage);
                }
            }
        }

    }
}
