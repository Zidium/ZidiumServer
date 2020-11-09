using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;
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
            var metricInfo = TestHelper.CreateTestMetricType(account.Id);
            var component = account.CreateTestApplicationComponent();

            InitMetricTypeRules(account.Id, metricInfo.Id);

            // Проверим красный цвет
            var date = DateTime.Now;
            var responseSend = client.ApiService.SendMetrics(component.Id, new List<Zidium.Api.SendMetricData>()
            {
                new Zidium.Api.SendMetricData()
                {
                    Name = metricInfo.SystemName,
                    Value = 10,
                    ActualInterval = TimeSpan.FromDays(1)
                }
            });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Id, metricInfo.SystemName, Zidium.Api.MonitoringStatus.Alarm);
        }

        [Fact]
        public void MetricYellowRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var metricInfo = TestHelper.CreateTestMetricType(account.Id);
            var component = account.CreateRandomComponentControl();

            InitMetricTypeRules(account.Id, metricInfo.Id);

            // Проверим жёлтый цвет
            var responseSend = component.SendMetric(new Zidium.Api.SendMetricData()
            {
                Name = metricInfo.SystemName,
                Value = 100,
                ActualInterval = TimeSpan.FromDays(1)
            });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, metricInfo.SystemName, Zidium.Api.MonitoringStatus.Warning);
        }

        [Fact]
        public void MetricGreenRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var metricInfo = TestHelper.CreateTestMetricType(account.Id);
            var component = account.CreateRandomComponentControl();

            InitMetricTypeRules(account.Id, metricInfo.Id);

            // Проверим зелёный цвет
            var responseSend = component.SendMetric(
                new Zidium.Api.SendMetricData()
                {
                    Name = metricInfo.SystemName,
                    Value = 1000,
                    ActualInterval = TimeSpan.FromDays(1)
                });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, metricInfo.SystemName, Zidium.Api.MonitoringStatus.Success);
        }

        [Fact]
        public void MetricElseRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var metricInfo = TestHelper.CreateTestMetricType(account.Id);
            var component = account.CreateRandomComponentControl();

            InitMetricTypeRules(account.Id, metricInfo.Id);

            // Проверим серый цвет
            var responseSend = component.SendMetric(new Zidium.Api.SendMetricData()
            {
                Name = metricInfo.SystemName,
                Value = 1001,
                ActualInterval = TimeSpan.FromDays(1)
            });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, metricInfo.SystemName, Zidium.Api.MonitoringStatus.Unknown);
        }

        [Fact]
        public void ComponentMetricRedRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var metric = TestHelper.CreateTestMetric(account.Id, component.Info.Id);
            var metricType = account.GetMetricTypeCache(metric.MetricTypeId);

            InitMetricRules(account.Id, metric.Id);

            // Проверим красный цвет
            var responseSend = component.SendMetric(
                new Zidium.Api.SendMetricData()
                {
                    Name = metricType.SystemName,
                    Value = 10,
                    ActualInterval = TimeSpan.FromDays(1)
                });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, metricType.SystemName, Zidium.Api.MonitoringStatus.Alarm);
        }

        [Fact]
        public void ComponentMetricYellowRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var metric = TestHelper.CreateTestMetric(account.Id, component.Info.Id);
            var metricType = account.GetMetricTypeCache(metric.MetricTypeId);

            InitMetricRules(account.Id, metric.Id);

            // Проверим красный цвет
            var responseSend = component.SendMetric(new Zidium.Api.SendMetricData()
            {
                Name = metricType.SystemName,
                Value = 100,
                ActualInterval = TimeSpan.FromDays(1)
            });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, metricType.SystemName, Zidium.Api.MonitoringStatus.Warning);
        }

        [Fact]
        public void ComponentMetricGreenRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var metric = TestHelper.CreateTestMetric(account.Id, component.Info.Id);
            var metricType = account.GetMetricTypeCache(metric.MetricTypeId);

            InitMetricRules(account.Id, metric.Id);

            // Проверим зеленый цвет
            var responseSend = component.SendMetric(
                new Zidium.Api.SendMetricData()
                {
                    Name = metricType.SystemName,
                    Value = 1000,
                    ActualInterval = TimeSpan.FromDays(1)
                });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, metricType.SystemName, Zidium.Api.MonitoringStatus.Success);
        }

        [Fact]
        public void ComponentMetricElseRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var metric = TestHelper.CreateTestMetric(account.Id, component.Info.Id);
            var metricType = account.GetMetricTypeCache(metric.MetricTypeId);

            InitMetricRules(account.Id, metric.Id);

            // Проверим серый цвет
            var responseSend = component.SendMetric(new Zidium.Api.SendMetricData()
            {
                Name = metricType.SystemName,
                Value = 1001,
                ActualInterval = TimeSpan.FromDays(1)
            });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, metricType.SystemName, Zidium.Api.MonitoringStatus.Unknown);
        }

        [Fact]
        public void OutdatedMetricValueTest()
        {
            var account = TestHelper.GetTestAccount();
            var metricType = TestHelper.CreateTestMetricType(account.Id);
            var component = account.CreateRandomComponentControl();

            InitMetricTypeRules(account.Id, metricType.Id);

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
            Assert.NotNull(getMetricResponse.Data);
            Assert.Equal(Zidium.Api.MonitoringStatus.Success, getMetricResponse.Data.Status);

            // Подождём 10 секунд
            Thread.Sleep(10 * 1000);

            // Теперь метрика должна стать красной
            getMetricResponse = component.GetMetric(metricType.SystemName);
            Assert.True(getMetricResponse.Success);
            Assert.NotNull(getMetricResponse.Data);
            Assert.Equal(Zidium.Api.MonitoringStatus.Alarm, getMetricResponse.Data.Status);
            Assert.Null(getMetricResponse.Data.Value);
        }

        [Fact]
        public void OutdatedMetricValueBackTest()
        {
            var account = TestHelper.GetTestAccount();
            var metricType = TestHelper.CreateTestMetricType(account.Id);
            var component = account.CreateRandomComponentControl();

            InitMetricTypeRules(account.Id, metricType.Id);

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
            Assert.NotNull(getMetricResponse.Data);
            Assert.Equal(Zidium.Api.MonitoringStatus.Alarm, getMetricResponse.Data.Status);
            Assert.Null(getMetricResponse.Data.Value);

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
            Assert.NotNull(getMetricResponse.Data);
            Assert.Equal(Zidium.Api.MonitoringStatus.Success, getMetricResponse.Data.Status);
            Assert.Equal(500, getMetricResponse.Data.Value);
        }

        protected void InitMetricTypeRules(Guid accountId, Guid metricTypeId)
        {
            // Получим тип метрики и заполним ее правила
            using (var accountContext = TestHelper.GetAccountDbContext(accountId))
            {
                var metricType = accountContext.MetricTypes.Find(metricTypeId);
                
                metricType.ConditionAlarm = "value <= 10";
                metricType.ConditionWarning = "value <= 100";
                metricType.ConditionSuccess = "value <= 1000";
                metricType.ConditionElseColor = ObjectColor.Gray;
                accountContext.SaveChanges();

                AllCaches.MetricTypes.Unload(new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = metricTypeId
                });
            }
        }

        protected void InitMetricRules(Guid accountId, Guid metricId)
        {
            // Получим метрику и заполним ее правила
            using (var accountContext = TestHelper.GetAccountDbContext(accountId))
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
                metric.ConditionWarning= "value <= 100";
                metric.ConditionSuccess = "value <= 1000";
                metric.ConditionElseColor = ObjectColor.Gray;
                accountContext.SaveChanges();

                AllCaches.MetricTypes.Unload(new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = metricType.Id
                });
                AllCaches.Metrics.Unload(new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = metric.Id
                });
            }
        }

        protected void CheckMetricColor(Zidium.Api.IClient client, Guid componentId, string metricName, Zidium.Api.MonitoringStatus status)
        {
            // Проверим цвет в истории
            var historyResponse = client.ApiService.GetMetricsHistory(componentId, new Zidium.Api.GetMetricsHistoryFilter()
            {
                MaxCount = 100
            });
            Assert.True(historyResponse.Success);
            var history = historyResponse.Data.Where(t => t.Name == metricName).OrderByDescending(t => t.BeginDate).FirstOrDefault();
            Assert.NotNull(history);
            Assert.Equal(status, history.Status);

            // Проверим цвет текущего значения
            var getMetricResponse = client.ApiService.GetMetric(componentId, metricName);
            Assert.True(getMetricResponse.Success);
            Assert.NotNull(getMetricResponse.Data);
            Assert.Equal(status, getMetricResponse.Data.Status);

            var getComponentResponse = client.ApiService.GetComponentInternalState(componentId, false);
            Assert.True(getComponentResponse.Success);
            Assert.Equal(status, getComponentResponse.Data.Status);
        }

        [Fact]
        public void ChangeNoSignalColorTest()
        {
            var account = TestHelper.GetTestAccount();
            var metricType = TestHelper.CreateTestMetricType(account.Id);
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
            Assert.NotNull(getMetricResponse.Data);
            Assert.Equal(Zidium.Api.MonitoringStatus.Alarm, getMetricResponse.Data.Status);

            var metric = dispatcher.GetMetric(account.Id, component.Info.Id, metricType.SystemName).Data;

            // Изменим "Цвет если нет сигнала" на жёлтый
            dispatcher.UpdateMetric(account.Id, new UpdateMetricRequestData()
            {
                MetricId = metric.Id,
                NoSignalColor = ObjectColor.Yellow
            }).Check();

            // Проверим, что метрика стала жёлтой
            getMetricResponse = component.GetMetric(metricType.SystemName);
            Assert.True(getMetricResponse.Success);
            Assert.NotNull(getMetricResponse.Data);
            Assert.Equal(Zidium.Api.MonitoringStatus.Warning, getMetricResponse.Data.Status);
        }
    }
}
