using System;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Zidium.Agent.AgentTasks.DeleteMetricHistory;
using Zidium.Api;
using Zidium.Api.Dto;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Agent.Single.Tests
{
    public class DeleteMetricHistoryTests : BaseTest
    {
        [Fact]
        public void DeleteMetricHistoryTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var metric = TestHelper.CreateTestMetric(component.Info.Id);

            // Выполним предварительную очистку истории метрик
            var processor = new DeleteMetricHistoryProcessor(NullLogger.Instance, new CancellationToken());
            processor.Process();

            // Создадим одно старое значение метрики и одно новое
            var now = DateTime.Now.Date;
            using (var context = account.GetDbContext())
            {
                context.MetricHistories.Add(new DbMetricHistory()
                {
                    Id = Guid.NewGuid(),
                    ComponentId = component.Info.Id,
                    MetricTypeId = metric.MetricTypeId,
                    BeginDate = now.AddDays(-31),
                    ActualDate = now.AddDays(-29),
                    Color = ObjectColor.Gray,
                    Value = 100
                });

                context.MetricHistories.Add(new DbMetricHistory()
                {
                    Id = Guid.NewGuid(),
                    ComponentId = component.Info.Id,
                    MetricTypeId = metric.MetricTypeId,
                    BeginDate = now.AddDays(-29),
                    ActualDate = now,
                    Color = ObjectColor.Gray,
                    Value = 200
                });

                context.SaveChanges();
            }

            // Проверим, что в истории два значения
            string metricName;
            using (var context = TestHelper.GetDbContext())
            {
                var metricType = context.MetricTypes.Find(metric.MetricTypeId);
                metricName = metricType.SystemName;
            }

            var metricHistory = component.GetMetricsHistory(new GetMetricsHistoryFilter()
            {
                Name = metricName
            }).Data;

            Assert.Equal(2, metricHistory.Count);

            // Удалим старые значения
            processor.Process();
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
