﻿using System.Threading;
using NLog;
using Zidium.Core.Common;
using Zidium.Core.ConfigDb;

namespace Zidium.Agent.AgentTasks.OutdatedUnitTests
{
    /// <summary>
    /// Обновляет устаревшие статусы проверок
    /// </summary>
    public class OutdatedUnitTestsProcessor
    {
        protected ILogger Logger;

        public MultipleDataBaseProcessor DbProcessor { get; set; }

        public int Count;

        public OutdatedUnitTestsProcessor(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            DbProcessor = new MultipleDataBaseProcessor(logger, cancellationToken);
        }

        public void Process()
        {
            DbProcessor.ForEachAccount(data =>
            {
                if (data.Account.Type != AccountType.Test)
                    ProcessAccount(data);
            });
            if (Count > 0)
                Logger.Info("Обновлено {0} статусов проверок", Count);
        }

        public void ProcessAccount(ForEachAccountData data)
        {
            var dispatcher = AgentHelper.GetDispatcherClient();
            while (true)
            {
                data.CancellationToken.ThrowIfCancellationRequested();

                const int maxCount = 100;
                var response = dispatcher.RecalcUnitTestsResults(data.Account.Id, maxCount);
                if (response.Success)
                {
                    var updateCount = response.Data.UpdateCount;
                    Interlocked.Add(ref Count, updateCount);
                    if (updateCount == 0)
                    {
                        if (data.Logger.IsTraceEnabled)
                            data.Logger.Trace("Обновлено статусов проверок: " + updateCount);
                        return;
                    }
                    if (data.Logger.IsDebugEnabled)
                        data.Logger.Debug("Обновлено статусов проверок: " + updateCount);
                    if (updateCount < maxCount)
                    {
                        return;
                    }
                }
                else
                {
                    data.Logger.Error("Ошибка: " + response.ErrorMessage);
                    return;
                }
            }
        }
    }
}
