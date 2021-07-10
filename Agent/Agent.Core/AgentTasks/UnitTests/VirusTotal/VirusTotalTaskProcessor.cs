using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Zidium.Agent.AgentTasks.UnitTests.VirusTotal.Processor;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.Common.TimeService;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal
{
    public class VirusTotalTaskProcessor : UnitTestProcessorBase
    {
        private readonly VirusTotalProcessor _processor;

        public VirusTotalTaskProcessor(ILogger logger, CancellationToken cancellationToken, ITimeService timeService)
            : base(logger, cancellationToken, timeService)
        {
            _processor = new VirusTotalProcessor(new VirusTotalLimitManager(), new TimeService(), logger);
        }

        private void FixInputData(VirusTotalProcessorInputData inputData)
        {
            // Если вдруг при создании проверки не установили шаг
            if (inputData.NextStep == 0)
            {
                inputData.NextStep = VirusTotalStep.Scan;
            }
        }

        protected override UnitTestExecutionInfo GetResult(
            IStorage storage,
            UnitTestForRead unitTest)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var rule = storage.UnitTestVirusTotalRules.GetOneOrNullByUnitTestId(unitTest.Id);
            if (rule == null)
            {
                return null; // Правило ещё не сохранено
            }

            var settingService = new AccountSettingService(storage);
            var inputData = new VirusTotalProcessorInputData()
            {
                ApiKey = settingService.VirusTotalApiKey,
                Url = rule.Url,
                ScanId = rule.ScanId,
                NextStep = rule.NextStep,
                ScanTime = rule.ScanTime,
                AttempCount = unitTest.AttempCount
            };
            FixInputData(inputData);

            try
            {
                var outputData = _processor.Process(inputData);

                var ruleForUpdate = rule.GetForUpdate();
                ruleForUpdate.NextStep.Set(outputData.NextStep);
                ruleForUpdate.ScanId.Set(outputData.ScanId);
                ruleForUpdate.ScanTime.Set(outputData.ScanTime);
                if (outputData.Result != null && outputData.ErrorCode.HasValue)
                {
                    ruleForUpdate.LastRunErrorCode.Set(outputData.ErrorCode);
                }
                storage.UnitTestVirusTotalRules.Update(ruleForUpdate);

                return new UnitTestExecutionInfo()
                {
                    ResultRequest = outputData.Result,
                    NextStepProcessTime = outputData.NextStepProcessTime
                };
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);

                return new UnitTestExecutionInfo()
                {
                    IsNetworkProblem = true
                };
            }
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestType.VirusTotalTestType.Id;
        }
    }
}
