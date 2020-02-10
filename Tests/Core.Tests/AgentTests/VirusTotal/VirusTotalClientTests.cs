using System;
using System.Configuration;
using Xunit;
using Zidium.Agent.AgentTasks.UnitTests.VirusTotal.Client;

namespace Zidium.Core.Tests.AgentTests
{
    public class VirusTotalClientTests
    {
        private readonly string _apiKey;

        public VirusTotalClientTests()
        {
            _apiKey = ConfigurationManager.AppSettings["VirusTotalKey"];
        }

        private void SleepLimits()
        {
            VirusTotalProcessorTests.limitManager.SleepByLimits(_apiKey);
        }

        [Fact]
        public void ScanTest()
        {
            IVirusTotalClient client = new VirusTotalClient();
            SleepLimits();
            var response = client.Scan(new ScanRequest()
            {
                Apikey = _apiKey,
                Url = "http://recursion.ru"
            });
            Assert.Equal(1, response.response_code);
            Assert.NotNull(response.scan_id);
        }

        [Fact]
        public void ReportTest()
        {
            // получаем scan_id
            IVirusTotalClient client = new VirusTotalClient();
            SleepLimits();
            var response1 = client.Scan(new ScanRequest()
            {
                Apikey = _apiKey,
                Url = "http://recursion.ru"
            });
            Assert.Equal(1, response1.response_code);
            Assert.NotNull(response1.scan_id);

            // report
            SleepLimits();
            var response2 = client.Report(new ReportRequest()
            {
                Apikey = _apiKey,
                ScanId = response1.scan_id,
                Resource = response1.url
            });
            Assert.Equal(1, response2.response_code);
        }

        [Fact(Skip = "Используется для отладки")]
        public void LimitTest()
        {
            IVirusTotalClient client = new VirusTotalClient();
            int count = 10;
            for (int i = 0; i < count; i++)
            {
                var response1 = client.Scan(new ScanRequest()
                {
                    Apikey = _apiKey,
                    Url = "http://fake.domain.recursion.ru" // несуществующий домен не приводит к ошибке )
                });
                Assert.Equal(1, response1.response_code);
                Assert.NotNull(response1.scan_id);
            }
        }

        [Fact]
        public void FirstTimeTest()
        {
            IVirusTotalClient client = new VirusTotalClient();
            var url = "http://recursion.ru?fake=" + DateTime.Now.Ticks;
            SleepLimits();
            var response = client.Scan(new ScanRequest()
            {
                Apikey = _apiKey,
                Url = url
            });
            Assert.Equal(1, response.response_code);
            Assert.NotNull(response.scan_id);

            // 1-ый запрос отчета
            SleepLimits();
            var reportResponse1 = client.Report(new ReportRequest()
            {
                Apikey = _apiKey,
                Resource = url,
                ScanId = response.scan_id
            });
            Assert.Equal(response.scan_id, reportResponse1.scan_id);

            // 2-ый запрос отчета
            SleepLimits();
            var reportResponse2 = client.Report(new ReportRequest()
            {
                Apikey = _apiKey,
                Resource = url,
                ScanId = response.scan_id
            });
            Assert.Equal(response.scan_id, reportResponse2.scan_id);
            Assert.True(reportResponse2.total > 0);
        }
    }
}
