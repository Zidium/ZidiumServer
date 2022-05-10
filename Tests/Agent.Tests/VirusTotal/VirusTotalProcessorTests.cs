using System;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Zidium.Agent.AgentTasks.UnitTests.VirusTotal;
using Zidium.Agent.AgentTasks.UnitTests.VirusTotal.Processor;
using Zidium.Api.Dto;
using Zidium.Core.Common;
using Zidium.Storage;

namespace Zidium.Agent.Tests
{
    public class VirusTotalProcessorTests : BaseTest
    {
        private readonly string _apiKey;

        // используем общий для всех тестов процессор, чтобы не превысить лимиты АПИ
        public static VirusTotalLimitManager limitManager = new VirusTotalLimitManager();
        private VirusTotalProcessor processor;

        public VirusTotalProcessorTests()
        {
            var agentTestsConfiguration = DependencyInjection.GetServicePersistent<IAgentTestsConfiguration>();
            _apiKey = agentTestsConfiguration.VirusTotalKey;
            processor = new VirusTotalProcessor(limitManager, new TimeService(), NullLogger.Instance);
        }

        [Fact(Skip = "Integration")]
        public void UnknownApiKeyTest()
        {
            var output = processor.Process(new VirusTotalProcessorInputData()
            {
                ApiKey = "20938029830928009",
                Url = "http://recursion.ru",
                NextStep = VirusTotalStep.Scan
            });
            Assert.Equal(VirusTotalErrorCode.ServiceForbidden, output.ErrorCode);
            Assert.NotNull(output.Result);
            Assert.Equal(UnitTestResult.Alarm, output.Result.Result);
            Assert.Equal(VirusTotalStep.Scan, output.NextStep);
        }

        [Fact(Skip = "Integration")]
        public void SuccessTest()
        {
            // scan
            var url = "http://recursion.ru";
            var output = processor.Process(new VirusTotalProcessorInputData()
            {
                ApiKey = _apiKey,
                Url = url,
                NextStep = VirusTotalStep.Scan
            });
            Assert.Equal(VirusTotalStep.Report, output.NextStep);
            Assert.Null(output.Result);
            Assert.NotNull(output.NextStepProcessTime);
            Assert.NotNull(output.ScanId);
            Assert.NotNull(output.ScanTime);

            // report
            output = processor.Process(new VirusTotalProcessorInputData()
            {
                NextStep = VirusTotalStep.Report,
                ScanId = output.ScanId,
                ApiKey = _apiKey,
                Url = url,
                ScanTime = output.ScanTime.Value
            });
            Assert.Equal(VirusTotalStep.Scan, output.NextStep);
            Assert.NotNull(output.Result);
            Assert.Null(output.NextStepProcessTime);
            Assert.Null(output.ScanId);
        }

        /// <summary>
        /// Тест проверяет случай, когда url имеет недопустимый формат
        /// </summary>
        [Fact(Skip = "Integration")]
        public void InvalidUrlTest()
        {
            var output = processor.Process(new VirusTotalProcessorInputData()
            {
                ApiKey = _apiKey,
                Url = "http://lkfd lfkdlk", // содержит пробел
                NextStep = VirusTotalStep.Scan
            });
            Assert.Equal(VirusTotalErrorCode.VirusTotalApiError, output.ErrorCode);
            Assert.NotNull(output.Result);
            Assert.Equal(UnitTestResult.Alarm, output.Result.Result);
            Assert.Equal(VirusTotalStep.Scan, output.NextStep);
        }

        /// <summary>
        /// Тест проверяет случай, когда scan_id уже неактуален (неизвестен)
        /// </summary>
        [Fact(Skip = "Integration")]
        public void UnknownScanIdTest()
        {
            string url = "http://recursion.ru";
            var output = processor.Process(new VirusTotalProcessorInputData()
            {
                ApiKey = _apiKey,
                Url = url,
                NextStep = VirusTotalStep.Scan
            });
            output = processor.Process(new VirusTotalProcessorInputData()
            {
                ApiKey = _apiKey,
                Url = url,
                NextStep = VirusTotalStep.Report,
                ScanTime = output.ScanTime.Value.AddDays(-30),
                ScanId = "77d1d533bb517a3b04e42c9c5464926c9434ed95540504056ced96c2f9345f02-1580725111" // несуществующий
            });

            // все равно получим актуальный отчет, потому что scanId помоему не влияет
            Assert.Null(output.ScanId);
            Assert.Null(output.NextStepProcessTime);
            Assert.NotNull(output.Result);
            Assert.Equal(VirusTotalErrorCode.CleanSite, output.ErrorCode);
            Assert.Equal(VirusTotalStep.Scan, output.NextStep);
        }

        /// <summary>
        /// Тест проверяет случай, когда отчет неактуальный
        /// </summary>
        [Fact(Skip = "Integration")]
        public void NotActualReportTest()
        {
            string url = "http://recursion.ru";
            var output = processor.Process(new VirusTotalProcessorInputData()
            {
                ApiKey = _apiKey,
                Url = url,
                NextStep = VirusTotalStep.Scan
            });
            var scanTime = DateTime.Now.AddHours(1); // из будущего
            output = processor.Process(new VirusTotalProcessorInputData()
            {
                ApiKey = _apiKey,
                Url = url,
                NextStep = VirusTotalStep.Report,
                ScanId = output.ScanId,
                ScanTime = scanTime
            });

            // все равно получим актуальный отчет, потому что scanId помоему не влияет
            Assert.NotNull(output.ScanId);
            Assert.NotNull(output.NextStepProcessTime);
            Assert.Null(output.Result);
            Assert.Null(output.ErrorCode);
            Assert.Equal(VirusTotalStep.Report, output.NextStep);
            Assert.Equal(scanTime, output.ScanTime);
        }

        /// <summary>
        /// Тест проверяет случай, когда url ресурса изменился между шагами
        /// </summary>
        [Fact(Skip = "Integration")]
        public void ChangeUrlTest()
        {
            // scan
            string url = "http://recursion.ru";
            var output = processor.Process(new VirusTotalProcessorInputData()
            {
                ApiKey = _apiKey,
                Url = url,
                NextStep = VirusTotalStep.Scan
            });

            // report
            url = "http://recursion.ru?fake=" + DateTime.Now.Ticks; // изменили url между шагами
            output = processor.Process(new VirusTotalProcessorInputData()
            {
                ApiKey = _apiKey,
                Url = url,
                NextStep = VirusTotalStep.Report,
                ScanId = output.ScanId,
                ScanTime = output.ScanTime
            });

            // должны вернуться на шаг сканирования
            Assert.Null(output.ScanId);
            Assert.NotNull(output.NextStepProcessTime);
            Assert.Null(output.Result);
            Assert.Null(output.ErrorCode);
            Assert.Equal(VirusTotalStep.Scan, output.NextStep);
            Assert.Null(output.ScanTime);
        }
    }
}
