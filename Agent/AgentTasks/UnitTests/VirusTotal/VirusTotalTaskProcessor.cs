using System;
using System.Threading;
using NLog;
using Zidium.Agent.AgentTasks.UnitTests.VirusTotal.Processor;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.Common.TimeService;

namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal
{
    public class VirusTotalTaskProcessor : UnitTestProcessorBase
    {
        protected VirusTotalProcessor processor;

        public VirusTotalTaskProcessor(ILogger logger, CancellationToken cancellationToken, ITimeService timeService) 
            : base(logger, cancellationToken, timeService)
        {
            processor = new VirusTotalProcessor(new VirusTotalLimitManager(), new TimeService(), logger);
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
            Guid accountId, 
            AccountDbContext accountDbContext, 
            UnitTest unitTest, 
            ILogger logger, 
            string accountName, 
            CancellationToken token)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var rule = unitTest.VirusTotalRule;
            if (rule == null)
            {
                return null; // Правило ещё не сохранено
            }

            var settingService = accountDbContext.GetAccountSettingService();
            var inputData = new VirusTotalProcessorInputData()
            {
                ApiKey = settingService.VirusTotalApiKey,
                Url = rule.Url,
                ScanId = rule.ScanId,
                NextStep = rule.NextStep,
                ScanTime = rule.ScanTime
            };
            FixInputData(inputData);
            try
            {
                var outputData = processor.Process(inputData);
                rule.NextStep = outputData.NextStep;
                rule.ScanId = outputData.ScanId;
                rule.ScanTime = outputData.ScanTime;
                if (outputData.Result != null && outputData.ErrorCode.HasValue)
                {
                    rule.LastRunErrorCode = outputData.ErrorCode;
                }
                accountDbContext.SaveChanges();
                return new UnitTestExecutionInfo()
                {
                    ResultRequest = outputData.Result,
                    NextStepProcessTime = outputData.NextStepProcessTime
                };
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                return new UnitTestExecutionInfo()
                {
                    IsNetworkProblem = true
                };
            }
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestTypes.VirusTotalTestType.Id;
        }
    }
}
