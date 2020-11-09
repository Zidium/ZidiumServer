using System;
using System.Linq;
using System.Net;
using System.Threading;
using NLog;
using Zidium.Agent.AgentTasks.HttpRequests;
using Zidium.Core.AccountsDb;
using Xunit;
using Zidium.Agent.AgentTasks.UnitTests.HttpRequests;
using Zidium.Core.Common.Helpers;
using Zidium.Core.Common.TimeService;
using Zidium.Core.Tests.Services;
using Zidium.Storage;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Core.Tests.AgentTests
{
    public class HttpTestProcessorTests : BaseTest
    {
        [Fact]
        public void IsInternetWork()
        {
            Assert.True(NetworkHelper.IsInternetWork());
        }

        [Fact]
        public void UnknownDomainErrorTest()
        {
            HttpTestInputData inputData = new HttpTestInputData()
            {
                Url = "http://zidium-fake-777.com"
            };
            HttpTestProcessor processor = new HttpTestProcessor();
            var outputData = processor.Process(inputData);
            Assert.Equal(HttpRequestErrorCode.UnknownDomain, outputData.ErrorCode);
        }

        [Fact]
        public void SslTest1()
        {
            HttpTestInputData inputData = new HttpTestInputData()
            {
                Url = "https://www.eridon.ua/"
            };
            HttpTestProcessor processor = new HttpTestProcessor();
            var outputData = processor.Process(inputData);
            Assert.Equal(HttpRequestErrorCode.Success, outputData.ErrorCode);
        }

        [Fact(Skip = "Хз что за тест")]
        public void InfiniteHtmlTest()
        {
            HttpTestInputData inputData = new HttpTestInputData()
            {
                Url = "http://compalex.asmo-lenavto.infometeos.com/video.ashx",
                TimeoutSeconds = 5
            };
            HttpTestProcessor processor = new HttpTestProcessor();
            var outputData = processor.Process(inputData);
            Assert.Equal(HttpRequestErrorCode.Success, outputData.ErrorCode);
        }

        [Fact]
        public void IsDomainHasIpTest()
        {
            Assert.True(NetworkHelper.IsDomainHasIp("212.45.18.181"));
            Assert.True(NetworkHelper.IsDomainHasIp("google.ru"));
            Assert.True(NetworkHelper.IsDomainHasIp("recursion.ru"));
            Assert.False(NetworkHelper.IsDomainHasIp("fakesite22222.zidium.net"));
            Assert.False(NetworkHelper.IsDomainHasIp("sdsdsds.recursion.ru"));
            Assert.False(NetworkHelper.IsDomainHasIp("jhgjhgfjfeefef.ru"));
        }

        [Fact]
        public void SuccessCheckTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var unitTestType = client.GetOrCreateUnitTestTypeControl(SystemUnitTestType.HttpUnitTestType.SystemName);
            var unitTest = component.GetOrCreateUnitTestControl(unitTestType, "httpCheck." + Guid.NewGuid());
            var unitTestId = unitTest.Info.Id;

            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                unitTestObj.ErrorColor = UnitTestResult.Alarm;

                var httpUnitTest = unitTestObj.HttpRequestUnitTest;

                httpUnitTest.Rules.Add(new DbHttpRequestUnitTestRule()
                {
                    Id = Guid.NewGuid(),
                    HttpRequestUnitTest = httpUnitTest,
                    DisplayName = Guid.NewGuid().ToString(),
                    Url = "http://fakesite.zidium.net/ok",
                    TimeoutSeconds = 5
                });

                accountDbContext.SaveChanges();
            }

            var processor = new HttpRequestsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), new TimeService());
            processor.ProcessAccount(account.Id, unitTestId);
            Assert.Equal(1, processor.SuccessCount);
            Assert.Equal(0, processor.ErrorCount);

            // Проверка должна быть зелёной
            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                Assert.Equal(MonitoringStatus.Success, unitTestObj.Bulb.Status);
            }
        }

        [Fact]
        public void FailedCheckErrorHtmlFoundTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var unitTestType = client.GetOrCreateUnitTestTypeControl(SystemUnitTestType.HttpUnitTestType.SystemName);
            var unitTest = component.GetOrCreateUnitTestControl(unitTestType, "httpCheck." + Guid.NewGuid());
            var unitTestId = unitTest.Info.Id;

            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                unitTestObj.ErrorColor = UnitTestResult.Alarm;

                var httpUnitTest = unitTestObj.HttpRequestUnitTest;

                httpUnitTest.Rules.Add(new DbHttpRequestUnitTestRule()
                {
                    Id = Guid.NewGuid(),
                    HttpRequestUnitTest = httpUnitTest,
                    DisplayName = Guid.NewGuid().ToString(),
                    Url = "http://fakesite.zidium.net/ok",
                    ErrorHtml = "Все хорошо",
                    TimeoutSeconds = 5
                });

                accountDbContext.SaveChanges();
            }

            var processor = new HttpRequestsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), new TimeService());
            processor.ProcessAccount(account.Id, unitTestId);
            Assert.Equal(1, processor.SuccessCount);
            Assert.Equal(0, processor.ErrorCount);

            // Проверка должна быть красной
            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                Assert.Equal(MonitoringStatus.Alarm, unitTestObj.Bulb.Status);
                var rule = unitTestObj.HttpRequestUnitTest.Rules.First();
                Assert.Equal(HttpRequestErrorCode.ErrorHtmlFound, rule.LastRunErrorCode);
            }
        }

        [Fact]
        public void FailedCheckInvalidResponseCodeTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var unitTestType = client.GetOrCreateUnitTestTypeControl(SystemUnitTestType.HttpUnitTestType.SystemName);
            var unitTest = component.GetOrCreateUnitTestControl(unitTestType, "httpCheck." + Guid.NewGuid());
            var unitTestId = unitTest.Info.Id;

            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                unitTestObj.ErrorColor = UnitTestResult.Alarm;

                var httpUnitTest = unitTestObj.HttpRequestUnitTest;

                httpUnitTest.Rules.Add(new DbHttpRequestUnitTestRule()
                {
                    Id = Guid.NewGuid(),
                    HttpRequestUnitTest = httpUnitTest,
                    DisplayName = Guid.NewGuid().ToString(),
                    Url = "http://fakesite.zidium.net/ok-22222222",
                    TimeoutSeconds = 5
                });

                accountDbContext.SaveChanges();
            }

            var processor = new HttpRequestsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), new TimeService());
            processor.ProcessAccount(account.Id, unitTestId);
            Assert.Equal(1, processor.SuccessCount);
            Assert.Equal(0, processor.ErrorCount);

            // Проверка должна быть красной
            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                Assert.Equal(MonitoringStatus.Alarm, unitTestObj.Bulb.Status);
                var rule = unitTestObj.HttpRequestUnitTest.Rules.First();
                Assert.Equal(HttpRequestErrorCode.InvalidResponseCode, rule.LastRunErrorCode);
            }
        }

        [Fact]
        public void FailedCheckSuccessHtmlNotFoundTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var unitTestType = client.GetOrCreateUnitTestTypeControl(SystemUnitTestType.HttpUnitTestType.SystemName);
            var unitTest = component.GetOrCreateUnitTestControl(unitTestType, "httpCheck." + Guid.NewGuid());
            var unitTestId = unitTest.Info.Id;

            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                unitTestObj.ErrorColor = UnitTestResult.Alarm;

                var httpUnitTest = unitTestObj.HttpRequestUnitTest;

                httpUnitTest.Rules.Add(new DbHttpRequestUnitTestRule()
                {
                    Id = Guid.NewGuid(),
                    HttpRequestUnitTest = httpUnitTest,
                    DisplayName = Guid.NewGuid().ToString(),
                    Url = "http://fakesite.zidium.net/ok",
                    SuccessHtml = "Все хорошо 77777777",
                    TimeoutSeconds = 5
                });

                accountDbContext.SaveChanges();
            }

            var processor = new HttpRequestsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), new TimeService());
            processor.ProcessAccount(account.Id, unitTestId);
            Assert.Equal(1, processor.SuccessCount);
            Assert.Equal(0, processor.ErrorCount);

            // Проверка должна быть красной
            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                Assert.Equal(MonitoringStatus.Alarm, unitTestObj.Bulb.Status);
                var rule = unitTestObj.HttpRequestUnitTest.Rules.First();
                Assert.Equal(HttpRequestErrorCode.SuccessHtmlNotFound, rule.LastRunErrorCode);
            }
        }

        [Fact]
        public void FailedCheckTcpErrorTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var unitTestType = client.GetOrCreateUnitTestTypeControl(SystemUnitTestType.HttpUnitTestType.SystemName);
            var unitTest = component.GetOrCreateUnitTestControl(unitTestType, "httpCheck." + Guid.NewGuid());
            var unitTestId = unitTest.Info.Id;

            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                unitTestObj.ErrorColor = UnitTestResult.Alarm;

                var httpUnitTest = unitTestObj.HttpRequestUnitTest;

                httpUnitTest.Rules.Add(new DbHttpRequestUnitTestRule()
                {
                    Id = Guid.NewGuid(),
                    HttpRequestUnitTest = httpUnitTest,
                    DisplayName = Guid.NewGuid().ToString(),
                    Url = "http://localhost:3451/",
                    TimeoutSeconds = 5
                });

                accountDbContext.SaveChanges();
            }

            var processor = new HttpRequestsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), new TimeService());
            processor.ProcessAccount(account.Id, unitTestId);
            Assert.Equal(1, processor.SuccessCount);
            Assert.Equal(0, processor.ErrorCount);

            // Проверка должна быть красной
            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                Assert.Equal(MonitoringStatus.Alarm, unitTestObj.Bulb.Status);
                var rule = unitTestObj.HttpRequestUnitTest.Rules.First();
                Assert.Equal(HttpRequestErrorCode.TcpError, rule.LastRunErrorCode);
            }
        }

        [Fact]
        public void FailedCheckTimeoutTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var unitTestType = client.GetOrCreateUnitTestTypeControl(SystemUnitTestType.HttpUnitTestType.SystemName);
            var unitTest = component.GetOrCreateUnitTestControl(unitTestType, "httpCheck." + Guid.NewGuid());
            var unitTestId = unitTest.Info.Id;

            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                unitTestObj.ErrorColor = UnitTestResult.Alarm;

                var httpUnitTest = unitTestObj.HttpRequestUnitTest;

                httpUnitTest.Rules.Add(new DbHttpRequestUnitTestRule()
                {
                    Id = Guid.NewGuid(),
                    HttpRequestUnitTest = httpUnitTest,
                    DisplayName = Guid.NewGuid().ToString(),
                    Url = "http://fakesite.zidium.net/ok?wait=5",
                    TimeoutSeconds = 4
                });

                accountDbContext.SaveChanges();
            }

            var processor = new HttpRequestsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), new TimeService());
            processor.ProcessAccount(account.Id, unitTestId);
            Assert.Equal(1, processor.SuccessCount);
            Assert.Equal(0, processor.ErrorCount);

            // Проверка должна быть красной
            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                Assert.Equal(MonitoringStatus.Alarm, unitTestObj.Bulb.Status);
                var rule = unitTestObj.HttpRequestUnitTest.Rules.First();
                Assert.Equal(HttpRequestErrorCode.Timeout, rule.LastRunErrorCode);
            }
        }

        [Fact]
        public void FailedCheckTooLargeResponseTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var unitTestType = client.GetOrCreateUnitTestTypeControl(SystemUnitTestType.HttpUnitTestType.SystemName);
            var unitTest = component.GetOrCreateUnitTestControl(unitTestType, "httpCheck." + Guid.NewGuid());
            var unitTestId = unitTest.Info.Id;

            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                unitTestObj.ErrorColor = UnitTestResult.Alarm;

                var httpUnitTest = unitTestObj.HttpRequestUnitTest;

                httpUnitTest.Rules.Add(new DbHttpRequestUnitTestRule()
                {
                    Id = Guid.NewGuid(),
                    HttpRequestUnitTest = httpUnitTest,
                    DisplayName = Guid.NewGuid().ToString(),
                    Url = "http://fakesite.zidium.net/ok",
                    TimeoutSeconds = 5,
                    MaxResponseSize = 1
                });

                accountDbContext.SaveChanges();
            }

            var processor = new HttpRequestsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), new TimeService());
            processor.ProcessAccount(account.Id, unitTestId);
            Assert.Equal(1, processor.SuccessCount);
            Assert.Equal(0, processor.ErrorCount);

            // Проверка должна быть красной
            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                Assert.Equal(MonitoringStatus.Alarm, unitTestObj.Bulb.Status);
                var rule = unitTestObj.HttpRequestUnitTest.Rules.First();
                Assert.Equal(HttpRequestErrorCode.TooLargeResponse, rule.LastRunErrorCode);
            }
        }

        [Fact]
        public void FailedCheckUnknownDomainTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var unitTestType = client.GetOrCreateUnitTestTypeControl(SystemUnitTestType.HttpUnitTestType.SystemName);
            var unitTest = component.GetOrCreateUnitTestControl(unitTestType, "httpCheck." + Guid.NewGuid());
            var unitTestId = unitTest.Info.Id;

            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                unitTestObj.ErrorColor = UnitTestResult.Alarm;

                var httpUnitTest = unitTestObj.HttpRequestUnitTest;

                httpUnitTest.Rules.Add(new DbHttpRequestUnitTestRule()
                {
                    Id = Guid.NewGuid(),
                    HttpRequestUnitTest = httpUnitTest,
                    DisplayName = Guid.NewGuid().ToString(),
                    Url = "http://fakesite2222.zidium.net/ok",
                    TimeoutSeconds = 5
                });

                accountDbContext.SaveChanges();
            }

            var processor = new HttpRequestsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), new TimeService());
            processor.ProcessAccount(account.Id, unitTestId);
            Assert.Equal(1, processor.SuccessCount);
            Assert.Equal(0, processor.ErrorCount);

            // Проверка должна быть красной
            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                Assert.Equal(MonitoringStatus.Alarm, unitTestObj.Bulb.Status);
                var rule = unitTestObj.HttpRequestUnitTest.Rules.First();
                Assert.Equal(HttpRequestErrorCode.UnknownDomain, rule.LastRunErrorCode);
            }
        }

        [Fact]
        public void FailedCheckUrlFormatErrorTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var unitTestType = client.GetOrCreateUnitTestTypeControl(SystemUnitTestType.HttpUnitTestType.SystemName);
            var unitTest = component.GetOrCreateUnitTestControl(unitTestType, "httpCheck." + Guid.NewGuid());
            var unitTestId = unitTest.Info.Id;

            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                unitTestObj.ErrorColor = UnitTestResult.Alarm;

                var httpUnitTest = unitTestObj.HttpRequestUnitTest;

                httpUnitTest.Rules.Add(new DbHttpRequestUnitTestRule()
                {
                    Id = Guid.NewGuid(),
                    HttpRequestUnitTest = httpUnitTest,
                    DisplayName = Guid.NewGuid().ToString(),
                    Url = "http5://fakesite.zidium.net/ok",
                    TimeoutSeconds = 5
                });

                accountDbContext.SaveChanges();
            }

            var processor = new HttpRequestsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), new TimeService());
            processor.ProcessAccount(account.Id, unitTestId);
            Assert.Equal(1, processor.SuccessCount);
            Assert.Equal(0, processor.ErrorCount);

            // Проверка должна быть красной
            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                Assert.Equal(MonitoringStatus.Alarm, unitTestObj.Bulb.Status);
                var rule = unitTestObj.HttpRequestUnitTest.Rules.First();
                Assert.Equal(HttpRequestErrorCode.UrlFormatError, rule.LastRunErrorCode);
            }
        }

        [Fact]
        public void MultipleAttempsTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();

            // Создадим проверку, которая должна провалиться только после второй попытки
            var unitTestType = client.GetOrCreateUnitTestTypeControl(SystemUnitTestType.HttpUnitTestType.SystemName);
            var unitTest = component.GetOrCreateUnitTestControl(unitTestType, "httpCheck." + Guid.NewGuid());
            var unitTestId = unitTest.Info.Id;

            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                unitTestObj.PeriodSeconds = (int)TimeSpan.FromMinutes(1).TotalSeconds;
                unitTestObj.AttempMax = 2;
                unitTestObj.ErrorColor = UnitTestResult.Alarm;

                var httpUnitTest = unitTestObj.HttpRequestUnitTest;

                httpUnitTest.Rules.Add(new DbHttpRequestUnitTestRule()
                {
                    Id = Guid.NewGuid(),
                    DisplayName = Guid.NewGuid().ToString(),
                    HttpRequestUnitTest = httpUnitTest,
                    Url = "http://fakesite.zidium.net/ok?wait=10",
                    TimeoutSeconds = 5
                });

                accountDbContext.SaveChanges();
            }

            // Первая попытка
            FixedTimeService timeService = new FixedTimeService();
            var processor = new HttpRequestsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), timeService);
            processor.ProcessAccount(account.Id, unitTestId);
            Assert.Null(processor.DbProcessor.FirstException);
            Assert.Equal(1, processor.SuccessCount);
            Assert.Equal(0, processor.ErrorCount);

            // Проверка не должна стать красной
            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                Assert.Equal(1, unitTestObj.AttempCount);
                Assert.Equal(MonitoringStatus.Unknown, unitTestObj.Bulb.Status);
            }

            // Вторая попытка
            processor = new HttpRequestsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), timeService);
            timeService.Set(DateTime.Now.AddMinutes(1));
            processor.ProcessAccount(account.Id, unitTestId);
            Assert.Null(processor.DbProcessor.FirstException);
            Assert.Equal(1, processor.SuccessCount);
            Assert.Equal(0, processor.ErrorCount);

            // Теперь проверка должна быть красной
            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                Assert.Equal(2, unitTestObj.AttempCount);
                Assert.Equal(MonitoringStatus.Alarm, unitTestObj.Bulb.Status);
            }
        }

        [Fact]
        public void CookieTest()
        {
            HttpTestInputData inputData = new HttpTestInputData()
            {
                Url = "https://zidium.net",
                Method = HttpRequestMethod.Get,
                Cookies = new CookieCollection()
                {
                    new Cookie("sessionid", "12345")
                }
            };
            HttpTestProcessor processor = new HttpTestProcessor();
            var outputData = processor.Process(inputData);
            Assert.Equal(HttpRequestErrorCode.Success, outputData.ErrorCode);
        }
    }
}
