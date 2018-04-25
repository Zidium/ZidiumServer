using System;
using System.Threading;
using NLog;
using Xunit;
using Zidium.Agent.AgentTasks.DeleteMetricHistory;
using Zidium.Api;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.TestTools;

namespace Zidium.Core.Single.Tests
{
    public class DeleteMetricHistoryTests
    {
        [Fact]
        public void DeleteMetricHistoryTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var metric = TestHelper.CreateTestMetric(account.Id, component.Info.Id);

            // Укажем время хранения истории метрик
            var dispatcher = DispatcherHelper.GetDispatcherService();
            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data.Hard;
            limits.MetricsMaxDays = 10;
            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits,
                    Type = TariffLimitType.Hard
                }
            }).Check();

            // Выполним предварительную очистку истории метрик
            var processor = new DeleteMetricHistoryProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.Process(account.Id, component.Info.Id);
            Assert.Null(processor.DbProcessor.FirstException);

            // Создадим одно старое значение метрики и одно новое
            var now = DateTime.Now.Date;
            using (var context = account.CreateAccountDbContext())
            {
                var metricHistoryRepository = context.GetMetricHistoryRepository();

                metricHistoryRepository.Add(new MetricHistory()
                {
                    ComponentId = component.Info.Id,
                    MetricTypeId = metric.MetricTypeId,
                    BeginDate = now.AddDays(-20),
                    ActualDate = now.AddDays(-11),
                    Color = ObjectColor.Gray,
                    Value = 100
                });

                metricHistoryRepository.Add(new MetricHistory()
                {
                    ComponentId = component.Info.Id,
                    MetricTypeId = metric.MetricTypeId,
                    BeginDate = now.AddDays(-11),
                    ActualDate = now,
                    Color = ObjectColor.Gray,
                    Value = 200
                });

                context.SaveChanges();
            }

            // Проверим, что в истории два значения
            string metricName;
            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var metricTypeRepository = context.GetMetricTypeRepository();
                var metricType = metricTypeRepository.GetById(metric.MetricTypeId);
                metricName = metricType.SystemName;
            }

            var metricHistory = component.GetMetricsHistory(new GetMetricsHistoryFilter()
            {
                Name = metricName
            }).Data;

            Assert.Equal(2, metricHistory.Count);

            // Удалим старые значения
            processor.Process(account.Id, component.Info.Id);
            Assert.Null(processor.DbProcessor.FirstException);
            Assert.Equal(1, processor.DeletedMetricValueCount);

            // Проверим, что осталось только новое значение
            metricHistory = component.GetMetricsHistory(new GetMetricsHistoryFilter()
            {
                Name = metricName
            }).Data;

            Assert.Equal(1, metricHistory.Count);
            Assert.Equal(200, metricHistory[0].Value);
        }
    }
}
