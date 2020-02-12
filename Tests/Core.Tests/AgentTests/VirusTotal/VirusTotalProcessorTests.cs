using System;
using NLog;
using Xunit;
using Zidium.Agent.AgentTasks.UnitTests.VirusTotal;
using Zidium.Agent.AgentTasks.UnitTests.VirusTotal.Processor;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common.TimeService;

namespace Zidium.Core.Tests.AgentTests
{
    public class VirusTotalProcessorTests
    {
        private string API_KEY = "a66cbd372af4bb83f6cc9c23811fda2d261bf97317f27730bcca1dc2a65b0964";

        // используем общий для всех тестов процессор, чтобы не превысить лимиты АПИ
        public static VirusTotalLimitManager limitManager = new VirusTotalLimitManager();
        private ILogger logger = LogManager.GetCurrentClassLogger();
        private VirusTotalProcessor processor;

        public VirusTotalProcessorTests()
        {
            processor = new VirusTotalProcessor(limitManager, new TimeService(), logger);
        }

        [Fact]
        private void UnknownApiKeyTest()
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

        [Fact]
        private void SuccessTest()
        {
            // scan
            var url = "http://recursion.ru";
            var output = processor.Process(new VirusTotalProcessorInputData()
            {
                ApiKey = API_KEY,
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
                ApiKey = API_KEY,
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
        [Fact]
        private void InvalidUrlTest()
        {
            var output = processor.Process(new VirusTotalProcessorInputData()
            {
                ApiKey = API_KEY,
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
        [Fact]
        private void UnknownScanIdTest()
        {
            string url = "http://recursion.ru";
            var output = processor.Process(new VirusTotalProcessorInputData()
            {
                ApiKey = API_KEY,
                Url = url,
                NextStep = VirusTotalStep.Scan
            });
            output = processor.Process(new VirusTotalProcessorInputData()
            {
                ApiKey = API_KEY,
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
        [Fact]
        private void NotActualReportTest()
        {
            string url = "http://recursion.ru";
            var output = processor.Process(new VirusTotalProcessorInputData()
            {
                ApiKey = API_KEY,
                Url = url,
                NextStep = VirusTotalStep.Scan
            });
            var scanTime = DateTime.Now.AddHours(1); // из будущего
            output = processor.Process(new VirusTotalProcessorInputData()
            {
                ApiKey = API_KEY,
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
        [Fact]
        private void ChangeUrlTest()
        {
            // scan
            string url = "http://recursion.ru";
            var output = processor.Process(new VirusTotalProcessorInputData()
            {
                ApiKey = API_KEY,
                Url = url,
                NextStep = VirusTotalStep.Scan
            });

            // report
            url = "http://recursion.ru?fake=" + DateTime.Now.Ticks; // изменили url между шагами
            output = processor.Process(new VirusTotalProcessorInputData()
            {
                ApiKey = API_KEY,
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
