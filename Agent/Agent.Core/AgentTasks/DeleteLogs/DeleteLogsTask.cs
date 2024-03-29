﻿using System;
using Microsoft.Extensions.Logging;
using Zidium.Api.Dto;

namespace Zidium.Agent.AgentTasks.DeleteLogs
{
    public class DeleteLogsTask : AgentTaskBase
    {
        public DeleteLogsTask()
        {
            ExecutionPeriod = TimeSpan.FromHours(1);
            WaitOnErrorTime = TimeSpan.FromHours(1);

            var logicSettings = LogicSettingsCache.LogicSettings;
            Logger.LogInformation($"Удаляются логи старше {logicSettings.LogMaxDays} дней");
        }

        protected override AgentTaskResult Do()
        {
            var processor = new DeleteLogsProcessor(Logger, CancellationToken);
            processor.Process();

            var result = string.Format(
                "Удалено логов: {0}; удалено свойств: {1}",
                processor.DeletedLogsCount,
                processor.DeletedPropertiesCount);

            return new AgentTaskResult(UnitTestResult.Success, result);
        }

    }
}
