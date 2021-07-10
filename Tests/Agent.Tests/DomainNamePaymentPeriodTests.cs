using System;
using System.Linq;
using System.Threading;
using Zidium.Agent.AgentTasks;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.TimeService;
using Zidium.Storage;
using Zidium.Storage.Ef;
using Zidium.TestTools;
using Microsoft.Extensions.Logging.Abstractions;

namespace Zidium.Agent.Tests
{

    public class DomainNamePaymentPeriodTests : BaseTest
    {
        [Fact]
        public void CommonTest()
        {
            DomainNamePaymentPeriodCheckProcessor.ParseAnyFormatDate("14-aug-2016");

            var result = DomainNamePaymentPeriodCheckProcessor.GetPaymentDate("recursion.ru");
            Assert.Equal(DomainNamePaymentPeriodErrorCode.Success, result.Code);
            Assert.True(result.Date.HasValue);

            result = DomainNamePaymentPeriodCheckProcessor.GetPaymentDate("aps-market.com");
            Assert.Equal(DomainNamePaymentPeriodErrorCode.FreeDomain, result.Code);
            Assert.False(result.Date.HasValue);

            result = DomainNamePaymentPeriodCheckProcessor.GetPaymentDate("stackoverflow.com");
            Assert.Equal(DomainNamePaymentPeriodErrorCode.Success, result.Code);
            Assert.True(result.Date.HasValue);

            result = DomainNamePaymentPeriodCheckProcessor.GetPaymentDate("microsoft.com");
            Assert.Equal(DomainNamePaymentPeriodErrorCode.Success, result.Code);
            Assert.True(result.Date.HasValue);

            result = DomainNamePaymentPeriodCheckProcessor.GetPaymentDate("anekdotov.net");
            Assert.Equal(DomainNamePaymentPeriodErrorCode.Success, result.Code);
            Assert.True(result.Date.HasValue);

            result = DomainNamePaymentPeriodCheckProcessor.GetPaymentDate("recursion2.ru");
            Assert.Equal(DomainNamePaymentPeriodErrorCode.FreeDomain, result.Code);
            Assert.False(result.Date.HasValue);

            result = DomainNamePaymentPeriodCheckProcessor.GetPaymentDate("awarm.net");
            Assert.Equal(DomainNamePaymentPeriodErrorCode.Success, result.Code);
            Assert.True(result.Date.HasValue);

            result = DomainNamePaymentPeriodCheckProcessor.GetPaymentDate("ylluzzore.ru");
            Assert.Equal(DomainNamePaymentPeriodErrorCode.Success, result.Code);
            Assert.True(result.Date.HasValue);

            result = DomainNamePaymentPeriodCheckProcessor.GetPaymentDate("doc.alcospot.ru");
            Assert.Equal(DomainNamePaymentPeriodErrorCode.FreeDomain, result.Code);
            Assert.False(result.Date.HasValue);
        }

        [Fact]
        public void RuDomainTest()
        {
            var result = DomainNamePaymentPeriodCheckProcessor.GetPaymentDate("stelladent.ru");
            Assert.Equal(DomainNamePaymentPeriodErrorCode.Success, result.Code);
            Assert.True(result.Date.HasValue);
        }

        [Fact]
        public void ExampleDotComTest()
        {
            var result = DomainNamePaymentPeriodCheckProcessor.GetPaymentDate("example.com");
            Assert.Equal(DomainNamePaymentPeriodErrorCode.Success, result.Code);
            Assert.True(result.Date.HasValue);
        }

        [Fact]
        public void ZidiumDotNetTest()
        {
            var result = DomainNamePaymentPeriodCheckProcessor.GetPaymentDate("zidium.net");
            Assert.Equal(DomainNamePaymentPeriodErrorCode.Success, result.Code);
            Assert.True(result.Date.HasValue);
        }

        [Fact]
        public void UaWebsiteTest()
        {
            var result = DomainNamePaymentPeriodCheckProcessor.GetPaymentDate("ukraine.com.ua");
            Assert.Equal(DomainNamePaymentPeriodErrorCode.Success, result.Code);
            Assert.True(result.Date.HasValue);
        }

        [Fact]
        public void RfWebsiteTest()
        {
            var result = DomainNamePaymentPeriodCheckProcessor.GetPaymentDate("БЛАГОВЕСТ-МОСКВА.РФ");
            Assert.Equal(DomainNamePaymentPeriodErrorCode.Success, result.Code);
            Assert.True(result.Date.HasValue);
        }

        [Fact]
        public void RfWebsite2Test()
        {
            var result = DomainNamePaymentPeriodCheckProcessor.GetPaymentDate("БЛАГОВЕСТ.МОСКВА");
            Assert.Equal(DomainNamePaymentPeriodErrorCode.Success, result.Code);
            Assert.True(result.Date.HasValue);
        }

        [Fact]
        public void FullNameTest()
        {
            var result = DomainNamePaymentPeriodCheckProcessor.GetPaymentDate("http://zidium.net/");
            Assert.Equal(DomainNamePaymentPeriodErrorCode.Success, result.Code);
            Assert.True(result.Date.HasValue);
        }

        /// <summary>
        /// Проверим, что если период проверки 1 день, учитывается при расчёте NextTime проверки
        /// Был баг, когда проверки срока оплаты домена выполнялись постоянно.
        /// </summary>
        [Fact]
        public void PeriodTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            // создадим проверку доменного имени
            var unitTestType = account.GetClient().GetOrCreateUnitTestTypeControl(SystemUnitTestType.DomainNameTestType.SystemName);
            var unitTest = component.GetOrCreateUnitTestControl(unitTestType, "UnitTest." + DateTime.Now.Ticks);

            unitTest.GetState().Check();
            var unitTestId = unitTest.Info.Id;

            var client = TestHelper.GetDispatcherClient();
            client.UpdateUnitTest(new Core.Api.UpdateUnitTestRequestData()
            {
                UnitTestId = unitTestId,
                ComponentId = component.Info.Id,
                DisplayName = unitTest.Info.DisplayName,
                PeriodSeconds = (int)TimeSpan.FromDays(1).TotalSeconds
            }).Check();

            using (var accountDbContext = account.GetDbContext())
            {
                var domainNameRule = new DbUnitTestDomainNamePaymentPeriodRule()
                {
                    UnitTestId = unitTestId,
                    WarningDaysCount = 10,
                    AlarmDaysCount = 5,
                    Domain = "zidium.net"
                };
                var test = accountDbContext.UnitTests.First(x => x.Id == unitTestId);
                test.DomainNamePaymentPeriodRule = domainNameRule;
                domainNameRule.UnitTest = test;
                accountDbContext.SaveChanges();

                Assert.NotNull(test.NextExecutionDate);
            }

            // выполним проверку
            account.SaveAllCaches();
            var processor = new DomainNamePaymentPeriodCheckProcessor(NullLogger.Instance, new CancellationToken(), new TimeService());
            processor.Process(unitTestId);
            Assert.Equal(1, processor.SuccessCount);
            account.SaveAllCaches();

            // проверим дату след выполнения
            using (var accountDbContext = account.GetDbContext())
            {
                var test = accountDbContext.UnitTests.First(x => x.Id == unitTestId);
                Assert.NotNull(test.NextExecutionDate);
                Assert.True(test.NextExecutionDate > DateTime.Now.AddHours(23));
                TestHelper.AreEqual(test.NextExecutionDate.Value, DateTime.Now.AddDays(1), TimeSpan.FromMinutes(5));
            }
        }
    }
}
