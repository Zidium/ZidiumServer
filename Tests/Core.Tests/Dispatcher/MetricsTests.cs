using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;
using Zidium.Api.Dto;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Storage;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Dispatcher
{
    public class MetricsTests : BaseTest
    {
        [Fact]
        public void MetricRedRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var metricInfo = TestHelper.CreateTestMetricType();
            var component = account.CreateTestApplicationComponent();

            InitMetricTypeRules(metricInfo.Id);

            // Проверим красный цвет
            var responseSend = client.GetComponentControl(component.Id).SendMetrics(new List<Zidium.Api.SendMetricData>()
            {
                new Zidium.Api.SendMetricData()
                {
                    Name = metricInfo.SystemName,
                    Value = 10,
                    ActualInterval = TimeSpan.FromDays(1)
                }
            });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Id, metricInfo.SystemName, MonitoringStatus.Alarm);
        }

        [Fact]
        public void MetricYellowRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var metricInfo = TestHelper.CreateTestMetricType();
            var component = account.CreateRandomComponentControl();

            InitMetricTypeRules(metricInfo.Id);

            // Проверим жёлтый цвет
            var responseSend = component.SendMetric(new Zidium.Api.SendMetricData()
            {
                Name = metricInfo.SystemName,
                Value = 100,
                ActualInterval = TimeSpan.FromDays(1)
            });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, metricInfo.SystemName, MonitoringStatus.Warning);
        }

        [Fact]
        public void MetricGreenRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var metricInfo = TestHelper.CreateTestMetricType();
            var component = account.CreateRandomComponentControl();

            InitMetricTypeRules(metricInfo.Id);

            // Проверим зелёный цвет
            var responseSend = component.SendMetric(
                new Zidium.Api.SendMetricData()
                {
                    Name = metricInfo.SystemName,
                    Value = 1000,
                    ActualInterval = TimeSpan.FromDays(1)
                });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, metricInfo.SystemName, MonitoringStatus.Success);
        }

        [Fact]
        public void MetricElseRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var metricInfo = TestHelper.CreateTestMetricType();
            var component = account.CreateRandomComponentControl();

            InitMetricTypeRules(metricInfo.Id);

            // Проверим серый цвет
            var responseSend = component.SendMetric(new Zidium.Api.SendMetricData()
            {
                Name = metricInfo.SystemName,
                Value = 1001,
                ActualInterval = TimeSpan.FromDays(1)
            });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, metricInfo.SystemName, MonitoringStatus.Unknown);
        }

        [Fact]
        public void ComponentMetricRedRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var metric = TestHelper.CreateTestMetric(component.Info.Id);
            var metricType = account.GetMetricTypeCache(metric.MetricTypeId);

            InitMetricRules(metric.Id);

            // Проверим красный цвет
            var responseSend = component.SendMetric(
                new Zidium.Api.SendMetricData()
                {
                    Name = metricType.SystemName,
                    Value = 10,
                    ActualInterval = TimeSpan.FromDays(1)
                });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, metricType.SystemName, MonitoringStatus.Alarm);
        }

        [Fact]
        public void ComponentMetricYellowRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var metric = TestHelper.CreateTestMetric(component.Info.Id);
            var metricType = account.GetMetricTypeCache(metric.MetricTypeId);

            InitMetricRules(metric.Id);

            // Проверим красный цвет
            var responseSend = component.SendMetric(new Zidium.Api.SendMetricData()
            {
                Name = metricType.SystemName,
                Value = 100,
                ActualInterval = TimeSpan.FromDays(1)
            });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, metricType.SystemName, MonitoringStatus.Warning);
        }

        [Fact]
        public void ComponentMetricGreenRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var metric = TestHelper.CreateTestMetric(component.Info.Id);
            var metricType = account.GetMetricTypeCache(metric.MetricTypeId);

            InitMetricRules(metric.Id);

            // Проверим зеленый цвет
            var responseSend = component.SendMetric(
                new Zidium.Api.SendMetricData()
                {
                    Name = metricType.SystemName,
                    Value = 1000,
                    ActualInterval = TimeSpan.FromDays(1)
                });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, metricType.SystemName, MonitoringStatus.Success);
        }

        [Fact]
        public void ComponentMetricElseRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var metric = TestHelper.CreateTestMetric(component.Info.Id);
            var metricType = account.GetMetricTypeCache(metric.MetricTypeId);

            InitMetricRules(metric.Id);

            // Проверим серый цвет
            var responseSend = component.SendMetric(new Zidium.Api.SendMetricData()
            {
                Name = metricType.SystemName,
                Value = 1001,
                ActualInterval = TimeSpan.FromDays(1)
            });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, metricType.SystemName, MonitoringStatus.Unknown);
        }

        [Fact]
        public void OutdatedMetricValueTest()
        {
            var account = TestHelper.GetTestAccount();
            var metricType = TestHelper.CreateTestMetricType();
            var component = account.CreateRandomComponentControl();

            InitMetricTypeRules(metricType.Id);

            // Отправим метрику с актуальностью 5 секунд
            var responseSend = component.SendMetric(
                new Zidium.Api.SendMetricData()
                {
                    Name = metricType.SystemName,
                    Value = 1000,
                    ActualInterval = TimeSpan.FromSeconds(10)
                });
            Assert.True(responseSend.Success);

            // Проверим, что метрика зелёная
            var getMetricResponse = component.GetMetric(metricType.SystemName);
            Assert.True(getMetricResponse.Success);
            Assert.NotNull(getMetricResponse.GetDataAndCheck());
            Assert.Equal(MonitoringStatus.Success, getMetricResponse.GetDataAndCheck().Status);

            // Подождём 10 секунд
            Thread.Sleep(10 * 1000);

            // Теперь метрика должна стать красной
            getMetricResponse = component.GetMetric(metricType.SystemName);
            Assert.True(getMetricResponse.Success);
            Assert.NotNull(getMetricResponse.GetDataAndCheck());
            Assert.Equal(MonitoringStatus.Alarm, getMetricResponse.GetDataAndCheck().Status);
            Assert.Null(getMetricResponse.GetDataAndCheck().Value);
        }

        [Fact]
        public void OutdatedMetricValueBackTest()
        {
            var account = TestHelper.GetTestAccount();
            var metricType = TestHelper.CreateTestMetricType();
            var component = account.CreateRandomComponentControl();

            InitMetricTypeRules(metricType.Id);

            // Отправим метрику с актуальностью 1 секунда
            var responseSend = component.SendMetric(
                new Zidium.Api.SendMetricData()
                {
                    Name = metricType.SystemName,
                    Value = 1000,
                    ActualInterval = TimeSpan.FromSeconds(1)
                });
            Assert.True(responseSend.Success);

            // Подождём 5 секунд
            Thread.Sleep(5 * 1000);

            // Метрика должна стать красной
            var getMetricResponse = component.GetMetric(metricType.SystemName);
            Assert.True(getMetricResponse.Success);
            Assert.NotNull(getMetricResponse.GetDataAndCheck());
            Assert.Equal(MonitoringStatus.Alarm, getMetricResponse.GetDataAndCheck().Status);
            Assert.Null(getMetricResponse.GetDataAndCheck().Value);

            // Снова отправим метрику
            responseSend = component.SendMetric(
                new Zidium.Api.SendMetricData()
                {
                    Name = metricType.SystemName,
                    Value = 500,
                    ActualInterval = TimeSpan.FromMinutes(60)
                });
            responseSend.Check();

            // Проверим, что метрика зелёная
            getMetricResponse = component.GetMetric(metricType.SystemName);
            Assert.True(getMetricResponse.Success);
            Assert.NotNull(getMetricResponse.GetDataAndCheck());
            Assert.Equal(MonitoringStatus.Success, getMetricResponse.GetDataAndCheck().Status);
            Assert.Equal(500, getMetricResponse.GetDataAndCheck().Value);
        }

        protected void InitMetricTypeRules(Guid metricTypeId)
        {
            // Получим тип метрики и заполним ее правила
            using (var accountContext = TestHelper.GetDbContext())
            {
                var metricType = accountContext.MetricTypes.Find(metricTypeId);

                metricType.ConditionAlarm = "value <= 10";
                metricType.ConditionWarning = "value <= 100";
                metricType.ConditionSuccess = "value <= 1000";
                metricType.ConditionElseColor = ObjectColor.Gray;
                accountContext.SaveChanges();

                AllCaches.MetricTypes.Unload(new AccountCacheRequest()
                {
                    ObjectId = metricTypeId
                });
            }
        }

        protected void InitMetricRules(Guid metricId)
        {
            // Получим метрику и заполним ее правила
            using (var accountContext = TestHelper.GetDbContext())
            {
                // Заполним правила метрики
                var metric = accountContext.Metrics.Find(metricId);
                var metricType = metric.MetricType;

                metricType.ConditionAlarm = "value <= 1";
                metricType.ConditionWarning = "value <= 2";
                metricType.ConditionSuccess = "value <= 3";
                metricType.ConditionElseColor = ObjectColor.Red;
                accountContext.SaveChanges();

                // Перекроем их правилами метрики компонента
                metric.ConditionAlarm = "value <= 10";
                metric.ConditionWarning = "value <= 100";
                metric.ConditionSuccess = "value <= 1000";
                metric.ConditionElseColor = ObjectColor.Gray;
                accountContext.SaveChanges();

                AllCaches.MetricTypes.Unload(new AccountCacheRequest()
                {
                    ObjectId = metricType.Id
                });
                AllCaches.Metrics.Unload(new AccountCacheRequest()
                {
                    ObjectId = metric.Id
                });
            }
        }

        protected void CheckMetricColor(Zidium.Api.IClient client, Guid componentId, string metricName, MonitoringStatus status)
        {
            // Проверим цвет в истории
            var historyResponse = client.GetComponentControl(componentId).GetMetricsHistory(new Zidium.Api.GetMetricsHistoryFilter()
            {
                MaxCount = 100
            });
            Assert.True(historyResponse.Success);
            var history = historyResponse.GetDataAndCheck().Where(t => t.Name == metricName).OrderByDescending(t => t.BeginDate).FirstOrDefault();
            Assert.NotNull(history);
            Assert.Equal(status, history.Status);

            // Проверим цвет текущего значения
            var getMetricResponse = client.ApiService.GetMetric(componentId, metricName);
            Assert.True(getMetricResponse.Success);
            Assert.NotNull(getMetricResponse.GetDataAndCheck());
            Assert.Equal(status, getMetricResponse.GetDataAndCheck().Status);

            var getComponentResponse = client.ApiService.GetComponentInternalState(componentId, false);
            Assert.True(getComponentResponse.Success);
            Assert.Equal(status, getComponentResponse.GetDataAndCheck().Status);
        }

        [Fact]
        public void ChangeNoSignalColorTest()
        {
            var account = TestHelper.GetTestAccount();
            var metricType = TestHelper.CreateTestMetricType();
            var component = account.CreateRandomComponentControl();
            var dispatcher = TestHelper.GetDispatcherClient();

            // Отправим метрику с актуальностью 1 секунду
            var responseSend = component.SendMetric(
                new Zidium.Api.SendMetricData()
                {
                    Name = metricType.SystemName,
                    Value = 1000,
                    ActualInterval = TimeSpan.FromSeconds(1)
                });
            Assert.True(responseSend.Success);

            // Подождём 2 секунды
            Thread.Sleep(2 * 1000);

            // Проверим, что метрика красная
            var getMetricResponse = component.GetMetric(metricType.SystemName);
            Assert.True(getMetricResponse.Success);
            Assert.NotNull(getMetricResponse.GetDataAndCheck());
            Assert.Equal(MonitoringStatus.Alarm, getMetricResponse.GetDataAndCheck().Status);

            var metric = dispatcher.GetMetric(component.Info.Id, metricType.SystemName).GetDataAndCheck();

            // Изменим "Цвет если нет сигнала" на жёлтый
            dispatcher.UpdateMetric(new UpdateMetricRequestData()
            {
                MetricId = metric.Id,
                NoSignalColor = ObjectColor.Yellow
            }).Check();

            // Проверим, что метрика стала жёлтой
            getMetricResponse = component.GetMetric(metricType.SystemName);
            Assert.True(getMetricResponse.Success);
            Assert.NotNull(getMetricResponse.GetDataAndCheck());
            Assert.Equal(MonitoringStatus.Warning, getMetricResponse.GetDataAndCheck().Status);
        }
    }
}
