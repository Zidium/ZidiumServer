using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;
using Zidium.Api;
using Zidium.Core.AccountsDb;
using Zidium.Core.Caching;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Dispatcher
{
    
    public class MetricsRulesTests
    {
        [Fact]
        public void CounterRedRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var counterInfo = TestHelper.CreateTestMetricType(account.Id);
            var component = account.CreateTestApplicationComponent();

            InitMetricTypeRules(account.Id, counterInfo.Id);

            // Проверим красный цвет
            var date = DateTime.Now;
            var responseSend = client.ApiService.SendMetrics(component.Id, new List<SendMetricData>()
            {
                new SendMetricData()
                {
                    Name = counterInfo.SystemName,
                    Value = 10,
                    ActualInterval = TimeSpan.FromDays(1)
                }
            });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Id, counterInfo.SystemName, MonitoringStatus.Alarm);
        }

        [Fact]
        public void CounterYellowRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var counterInfo = TestHelper.CreateTestMetricType(account.Id);
            var component = account.CreateRandomComponentControl();

            InitMetricTypeRules(account.Id, counterInfo.Id);

            // Проверим жёлтый цвет
            var responseSend = component.SendMetric(new SendMetricData()
            {
                Name = counterInfo.SystemName,
                Value = 100,
                ActualInterval = TimeSpan.FromDays(1)
            });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, counterInfo.SystemName, MonitoringStatus.Warning);
        }

        [Fact]
        public void CounterGreenRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var counterInfo = TestHelper.CreateTestMetricType(account.Id);
            var component = account.CreateRandomComponentControl();

            InitMetricTypeRules(account.Id, counterInfo.Id);

            // Проверим зелёный цвет
            var responseSend = component.SendMetric(
                new SendMetricData()
                {
                    Name = counterInfo.SystemName,
                    Value = 1000,
                    ActualInterval = TimeSpan.FromDays(1)
                });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, counterInfo.SystemName, MonitoringStatus.Success);
        }

        [Fact]
        public void CounterElseRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var counterInfo = TestHelper.CreateTestMetricType(account.Id);
            var component = account.CreateRandomComponentControl();

            InitMetricTypeRules(account.Id, counterInfo.Id);

            // Проверим серый цвет
            var responseSend = component.SendMetric(new SendMetricData()
            {
                Name = counterInfo.SystemName,
                Value = 1001,
                ActualInterval = TimeSpan.FromDays(1)
            });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, counterInfo.SystemName, MonitoringStatus.Unknown);
        }

        [Fact]
        public void ComponentCounterRedRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var metric = TestHelper.CreateTestMetric(account.Id, component.Info.Id);
            var metricType = account.GetMetricTypeCache(metric.MetricTypeId);

            InitMetricRules(account.Id, metric.Id);

            // Проверим красный цвет
            var responseSend = component.SendMetric(
                new SendMetricData()
                {
                    Name = metricType.SystemName,
                    Value = 10,
                    ActualInterval = TimeSpan.FromDays(1)
                });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, metricType.SystemName, MonitoringStatus.Alarm);
        }

        [Fact]
        public void ComponentCounterYellowRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var metric = TestHelper.CreateTestMetric(account.Id, component.Info.Id);
            var metricType = account.GetMetricTypeCache(metric.MetricTypeId);

            InitMetricRules(account.Id, metric.Id);

            // Проверим красный цвет
            var responseSend = component.SendMetric(new SendMetricData()
            {
                Name = metricType.SystemName,
                Value = 100,
                ActualInterval = TimeSpan.FromDays(1)
            });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, metricType.SystemName, MonitoringStatus.Warning);
        }

        [Fact]
        public void ComponentCounterGreenRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var metric = TestHelper.CreateTestMetric(account.Id, component.Info.Id);
            var metricType = account.GetMetricTypeCache(metric.MetricTypeId);

            InitMetricRules(account.Id, metric.Id);

            // Проверим зеленый цвет
            var responseSend = component.SendMetric(
                new SendMetricData()
                {
                    Name = metricType.SystemName,
                    Value = 1000,
                    ActualInterval = TimeSpan.FromDays(1)
                });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, metricType.SystemName, MonitoringStatus.Success);
        }

        [Fact]
        public void ComponentCounterElseRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var metric = TestHelper.CreateTestMetric(account.Id, component.Info.Id);
            var metricType = account.GetMetricTypeCache(metric.MetricTypeId);

            InitMetricRules(account.Id, metric.Id);

            // Проверим серый цвет
            var responseSend = component.SendMetric(new SendMetricData()
            {
                Name = metricType.SystemName,
                Value = 1001,
                ActualInterval = TimeSpan.FromDays(1)
            });
            Assert.True(responseSend.Success);
            CheckMetricColor(client, component.Info.Id, metricType.SystemName, MonitoringStatus.Unknown);
        }

        [Fact]
        public void OutdatedCounterValueTest()
        {
            var account = TestHelper.GetTestAccount();
            var metricType = TestHelper.CreateTestMetricType(account.Id);
            var component = account.CreateRandomComponentControl();

            InitMetricTypeRules(account.Id, metricType.Id);

            // Отправим счётчик с актуальностью 5 секунд
            var responseSend = component.SendMetric(
                new SendMetricData()
                {
                    Name = metricType.SystemName,
                    Value = 1000,
                    ActualInterval = TimeSpan.FromSeconds(10)
                });
            Assert.True(responseSend.Success);

            // Проверим, что счётчик зелёный
            var getCounterResponse = component.GetMetric(metricType.SystemName);
            Assert.True(getCounterResponse.Success);
            Assert.NotNull(getCounterResponse.Data);
            Assert.Equal(MonitoringStatus.Success, getCounterResponse.Data.Status);

            // Подождём 5 секунд
            Thread.Sleep(10 * 1000);

            // Теперь счётчик должен стать красным
            getCounterResponse = component.GetMetric(metricType.SystemName);
            Assert.True(getCounterResponse.Success);
            Assert.NotNull(getCounterResponse.Data);
            Assert.Equal(MonitoringStatus.Alarm, getCounterResponse.Data.Status);
        }

        protected void InitMetricTypeRules(Guid accountId, Guid metricTypeId)
        {
            // Получим тип метрики и заполним ее правила
            using (var accountContext = AccountDbContext.CreateFromAccountId(accountId))
            {
                var metricTypeRepository = accountContext.GetMetricTypeRepository();
                var metricType = metricTypeRepository.GetById(metricTypeId);
                
                metricType.ConditionAlarm = "value <= 10";
                metricType.ConditionWarning = "value <= 100";
                metricType.ConditionSuccess = "value <= 1000";
                metricType.ConditionElseColor = Core.Common.ObjectColor.Gray;
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
            using (var accountContext = AccountDbContext.CreateFromAccountId(accountId))
            {
                // Заполним правила счётчика
                var metricRepository = accountContext.GetMetricRepository();
                var metric = metricRepository.GetById(metricId);
                var metricType = metric.MetricType;
               
                metricType.ConditionAlarm = "value <= 1";
                metricType.ConditionWarning = "value <= 2";
                metricType.ConditionSuccess = "value <= 3";
                metricType.ConditionElseColor = Core.Common.ObjectColor.Red;
                accountContext.SaveChanges();

                // Перекроем их правилами счётчика компонента
                metric.ConditionAlarm = "value <= 10";
                metric.ConditionWarning= "value <= 100";
                metric.ConditionSuccess = "value <= 1000";
                metric.ConditionElseColor = Core.Common.ObjectColor.Gray;
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

        protected void CheckMetricColor(IClient client, Guid componentId, string metricName, MonitoringStatus status)
        {
            // Проверим цвет в истории
            var historyResponse = client.ApiService.GetMetricsHistory(componentId, new GetMetricsHistoryFilter()
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
    }
}
