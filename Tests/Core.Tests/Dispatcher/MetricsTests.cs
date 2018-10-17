using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;
using Zidium.Api;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.TestTools;
using MonitoringStatus = Zidium.Api.MonitoringStatus;

namespace Zidium.Core.Tests.Dispatcher
{
    public class MetricsTests
    {
        [Fact]
        public void MetricRedRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var metricInfo = TestHelper.CreateTestMetricType(account.Id);
            var component = account.CreateTestApplicationComponent();

            InitMetricTypeRules(account.Id, metricInfo.Id);

            // �������� ������� ����
            var date = DateTime.Now;
            var responseSend = client.ApiService.SendMetrics(component.Id, new List<SendMetricData>()
            {
                new SendMetricData()
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
            var metricInfo = TestHelper.CreateTestMetricType(account.Id);
            var component = account.CreateRandomComponentControl();

            InitMetricTypeRules(account.Id, metricInfo.Id);

            // �������� ����� ����
            var responseSend = component.SendMetric(new SendMetricData()
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
            var metricInfo = TestHelper.CreateTestMetricType(account.Id);
            var component = account.CreateRandomComponentControl();

            InitMetricTypeRules(account.Id, metricInfo.Id);

            // �������� ������ ����
            var responseSend = component.SendMetric(
                new SendMetricData()
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
            var metricInfo = TestHelper.CreateTestMetricType(account.Id);
            var component = account.CreateRandomComponentControl();

            InitMetricTypeRules(account.Id, metricInfo.Id);

            // �������� ����� ����
            var responseSend = component.SendMetric(new SendMetricData()
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
            var metric = TestHelper.CreateTestMetric(account.Id, component.Info.Id);
            var metricType = account.GetMetricTypeCache(metric.MetricTypeId);

            InitMetricRules(account.Id, metric.Id);

            // �������� ������� ����
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
        public void ComponentMetricYellowRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var metric = TestHelper.CreateTestMetric(account.Id, component.Info.Id);
            var metricType = account.GetMetricTypeCache(metric.MetricTypeId);

            InitMetricRules(account.Id, metric.Id);

            // �������� ������� ����
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
        public void ComponentMetricGreenRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var metric = TestHelper.CreateTestMetric(account.Id, component.Info.Id);
            var metricType = account.GetMetricTypeCache(metric.MetricTypeId);

            InitMetricRules(account.Id, metric.Id);

            // �������� ������� ����
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
        public void ComponentMetricElseRuleTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var metric = TestHelper.CreateTestMetric(account.Id, component.Info.Id);
            var metricType = account.GetMetricTypeCache(metric.MetricTypeId);

            InitMetricRules(account.Id, metric.Id);

            // �������� ����� ����
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
        public void OutdatedMetricValueTest()
        {
            var account = TestHelper.GetTestAccount();
            var metricType = TestHelper.CreateTestMetricType(account.Id);
            var component = account.CreateRandomComponentControl();

            InitMetricTypeRules(account.Id, metricType.Id);

            // �������� ������� � ������������� 5 ������
            var responseSend = component.SendMetric(
                new SendMetricData()
                {
                    Name = metricType.SystemName,
                    Value = 1000,
                    ActualInterval = TimeSpan.FromSeconds(10)
                });
            Assert.True(responseSend.Success);

            // ��������, ��� ������� ������
            var getMetricResponse = component.GetMetric(metricType.SystemName);
            Assert.True(getMetricResponse.Success);
            Assert.NotNull(getMetricResponse.Data);
            Assert.Equal(MonitoringStatus.Success, getMetricResponse.Data.Status);

            // ������� 10 ������
            Thread.Sleep(10 * 1000);

            // ������ ������� ������ ����� �������
            getMetricResponse = component.GetMetric(metricType.SystemName);
            Assert.True(getMetricResponse.Success);
            Assert.NotNull(getMetricResponse.Data);
            Assert.Equal(MonitoringStatus.Alarm, getMetricResponse.Data.Status);
        }

        protected void InitMetricTypeRules(Guid accountId, Guid metricTypeId)
        {
            // ������� ��� ������� � �������� �� �������
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
            // ������� ������� � �������� �� �������
            using (var accountContext = AccountDbContext.CreateFromAccountId(accountId))
            {
                // �������� ������� �������
                var metricRepository = accountContext.GetMetricRepository();
                var metric = metricRepository.GetById(metricId);
                var metricType = metric.MetricType;
               
                metricType.ConditionAlarm = "value <= 1";
                metricType.ConditionWarning = "value <= 2";
                metricType.ConditionSuccess = "value <= 3";
                metricType.ConditionElseColor = Core.Common.ObjectColor.Red;
                accountContext.SaveChanges();

                // ��������� �� ��������� ������� ����������
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
            // �������� ���� � �������
            var historyResponse = client.ApiService.GetMetricsHistory(componentId, new GetMetricsHistoryFilter()
            {
                MaxCount = 100
            });
            Assert.True(historyResponse.Success);
            var history = historyResponse.Data.Where(t => t.Name == metricName).OrderByDescending(t => t.BeginDate).FirstOrDefault();
            Assert.NotNull(history);
            Assert.Equal(status, history.Status);

            // �������� ���� �������� ��������
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

            // �������� ������� � ������������� 1 �������
            var responseSend = component.SendMetric(
                new SendMetricData()
                {
                    Name = metricType.SystemName,
                    Value = 1000,
                    ActualInterval = TimeSpan.FromSeconds(1)
                });
            Assert.True(responseSend.Success);

            // ������� 2 �������
            Thread.Sleep(2 * 1000);

            // ��������, ��� ������� �������
            var getMetricResponse = component.GetMetric(metricType.SystemName);
            Assert.True(getMetricResponse.Success);
            Assert.NotNull(getMetricResponse.Data);
            Assert.Equal(MonitoringStatus.Alarm, getMetricResponse.Data.Status);

            var metric = dispatcher.GetMetric(account.Id, component.Info.Id, metricType.SystemName).Data;

            // ������� "���� ���� ��� �������" �� �����
            dispatcher.UpdateMetric(account.Id, new UpdateMetricRequestData()
            {
                MetricId = metric.Id,
                NoSignalColor = ObjectColor.Yellow
            }).Check();

            // ��������, ��� ������� ����� �����
            getMetricResponse = component.GetMetric(metricType.SystemName);
            Assert.True(getMetricResponse.Success);
            Assert.NotNull(getMetricResponse.Data);
            Assert.Equal(MonitoringStatus.Warning, getMetricResponse.Data.Status);
        }
    }
}
