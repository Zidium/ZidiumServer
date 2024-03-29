﻿using System;
using Zidium.Api.Dto;

namespace Zidium.Agent.AgentTasks.OutdatedUnitTests
{
    public class OutdatedUnitTestsTask : AgentTaskBase
    {
        public OutdatedUnitTestsTask()
        {
            ExecutionPeriod = TimeSpan.FromMinutes(1);
        }

        protected override AgentTaskResult Do()
        {
            var processor = new OutdatedUnitTestsProcessor(Logger, CancellationToken);
            processor.Process();

            var result = string.Format("Обновлено {0} статусов проверок", processor.Count);
            return new AgentTaskResult(UnitTestResult.Success, result);
        }
    }
}
