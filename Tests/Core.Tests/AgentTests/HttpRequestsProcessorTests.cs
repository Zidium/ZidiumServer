using System;
using System.Linq;
using System.Threading;
using NLog;
using Zidium.Agent.AgentTasks.HttpRequests;
using Zidium.Core.AccountsDb;
using Xunit;
using Zidium.Core.Common.Helpers;
using Zidium.TestTools;

namespace Zidium.Core.Tests.AgentTests
{
    public class HttpRequestsProcessorTests
    {
        [Fact]
        public void IsInternetWork()
        {
            Assert.True(NetworkHelper.IsInternetWork());
        }

        [Fact]
        public void IsIPTest()
        {
            Assert.True(NetworkHelper.IsIpAddress("212.45.18.181"));
            Assert.True(NetworkHelper.IsIpAddress("127.0.0.1"));
            Assert.False(NetworkHelper.IsIpAddress("256.45.18.181"));
            Assert.False(NetworkHelper.IsIpAddress("zidium.net"));
        }

        [Fact]
        public void IsDomainHasIpTest()
        {
            Assert.True(NetworkHelper.IsDomainHasIp("212.45.18.181"));
            Assert.True(NetworkHelper.IsDomainHasIp("google.ru"));
            Assert.True(NetworkHelper.IsDomainHasIp("recursion.ru"));
            Assert.False(NetworkHelper.IsDomainHasIp("fakesite22222.zidium.net"));
            Assert.True(NetworkHelper.IsDomainHasIp("asmo-m2.recursion.ru"));
            Assert.False(NetworkHelper.IsDomainHasIp("sdsdsds.recursion.ru"));
            Assert.False(NetworkHelper.IsDomainHasIp("jhgjhgfjfeefef.ru"));
        }

        [Fact]
        public void HttpRequestCheck()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var unitTestType = client.GetOrCreateUnitTestTypeControl(SystemUnitTestTypes.HttpUnitTestType.SystemName);
            var unitTest = component.GetOrCreateUnitTestControl(unitTestType, "httpCheck");
            Guid unitTestId = unitTest.Info.Id;

            using (var accountDbContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                var httpUnitTest = unitTestObj.HttpRequestUnitTest;

                var code = HttpRequestErrorCode.ErrorHtmlFound;
                httpUnitTest.Rules.Add(new HttpRequestUnitTestRule()
                {
                    DisplayName = code.ToString(),
                    Url = "http://fakesite.zidium.net/ok",
                    ErrorHtml = "Все хорошо",
                    TimeoutSeconds = 5
                });

                code = HttpRequestErrorCode.InvalidResponseCode;
                httpUnitTest.Rules.Add(new HttpRequestUnitTestRule()
                {
                    DisplayName = code.ToString(),
                    Url = "http://fakesite.zidium.net/ok-22222222",
                    TimeoutSeconds = 5
                });

                code = HttpRequestErrorCode.Success;
                httpUnitTest.Rules.Add(new HttpRequestUnitTestRule()
                {
                    DisplayName = code.ToString(),
                    Url = "http://fakesite.zidium.net/ok",
                    TimeoutSeconds = 5,
                    SuccessHtml = "Все хорошо"
                });

                code = HttpRequestErrorCode.SuccessHtmlNotFound;
                httpUnitTest.Rules.Add(new HttpRequestUnitTestRule()
                {
                    DisplayName = code.ToString(),
                    Url = "http://fakesite.zidium.net/ok",
                    TimeoutSeconds = 5,
                    SuccessHtml = "Все хорошо 77777777"
                });

                code = HttpRequestErrorCode.TcpError;
                httpUnitTest.Rules.Add(new HttpRequestUnitTestRule()
                {
                    DisplayName = code.ToString(),
                    Url = "http://localhost:3451/",
                    TimeoutSeconds = 60
                });

                code = HttpRequestErrorCode.Timeout;
                httpUnitTest.Rules.Add(new HttpRequestUnitTestRule()
                {
                    DisplayName = code.ToString(),
                    Url = "http://fakesite.zidium.net/ok?wait=5",
                    TimeoutSeconds = 4
                });

                code = HttpRequestErrorCode.TooLargeResponse;
                httpUnitTest.Rules.Add(new HttpRequestUnitTestRule()
                {
                    DisplayName = code.ToString(),
                    Url = "http://fakesite.zidium.net/ok",
                    TimeoutSeconds = 5,
                    MaxResponseSize = 1
                });

                code = HttpRequestErrorCode.UnknownDomain;
                httpUnitTest.Rules.Add(new HttpRequestUnitTestRule()
                {
                    DisplayName = code.ToString(),
                    Url = "http://fakesite22222.zidium.net/ok",
                    TimeoutSeconds = 5
                });

                code = HttpRequestErrorCode.UrlFormatError;
                httpUnitTest.Rules.Add(new HttpRequestUnitTestRule()
                {
                    DisplayName = code.ToString(),
                    Url = "http555555://fakesite.zidium.net/ok",
                    TimeoutSeconds = 5
                });

                int order = 0;
                foreach (var rule in httpUnitTest.Rules)
                {
                    rule.SortNumber = order++;
                    rule.HttpRequestUnitTest = httpUnitTest;
                    rule.HttpRequestUnitTestId = httpUnitTest.UnitTestId;
                    rule.Id = Guid.NewGuid();
                }
                httpUnitTest.ProcessAllRulesOnError = true;
                accountDbContext.SaveChanges();

                var processor = new HttpRequestsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
                processor.ProcessAccount(account.Id, unitTestId);
            }

            // проверим данные в БД
            using (var accountDbContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var test = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                Assert.True(test.NextExecutionDate > DateTime.Now);
                var codes = new[]
                {
                    HttpRequestErrorCode.ErrorHtmlFound,
                    HttpRequestErrorCode.InvalidResponseCode,
                    HttpRequestErrorCode.Success,
                    HttpRequestErrorCode.SuccessHtmlNotFound,
                    HttpRequestErrorCode.TcpError,
                    HttpRequestErrorCode.Timeout,
                    HttpRequestErrorCode.TooLargeResponse,
                    HttpRequestErrorCode.UnknownDomain,
                    HttpRequestErrorCode.UrlFormatError
                };
                foreach (var code in codes)
                {
                    var rule = test.HttpRequestUnitTest.Rules.Single(x => x.DisplayName == code.ToString());
                    Assert.Equal(code, rule.LastRunErrorCode);
                }
            }
        }

        [Fact]
        public void HttpProcessorTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var unitTestType = client.GetOrCreateUnitTestTypeControl(SystemUnitTestTypes.HttpUnitTestType.SystemName);
            var unitTest = component.GetOrCreateUnitTestControl(unitTestType, "httpCheck");
            Guid unitTestId = unitTest.Info.Id;

            using (var accountDbContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var unitTestObj = accountDbContext.UnitTests.Single(x => x.Id == unitTestId);
                var httpUnitTest = unitTestObj.HttpRequestUnitTest;

                var code = HttpRequestErrorCode.ErrorHtmlFound;
                httpUnitTest.Rules.Add(new HttpRequestUnitTestRule()
                {
                    DisplayName = code.ToString(),
                    Url = "http://fakesite.zidium.net/ok",
                    ErrorHtml = "Все хорошо",
                    TimeoutSeconds = 5
                });
                foreach (var rule in httpUnitTest.Rules)
                {
                    rule.HttpRequestUnitTest = httpUnitTest;
                    rule.HttpRequestUnitTestId = httpUnitTest.UnitTestId;
                    rule.Id = Guid.NewGuid();
                }

                accountDbContext.SaveChanges();

                var processor = new HttpRequestsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
                processor.ProcessAccount(account.Id, unitTestObj.Id);
                Assert.True(processor.SuccessCount >= 1);
                Assert.Equal(0, processor.ErrorCount);
            }
        }
    }
}
