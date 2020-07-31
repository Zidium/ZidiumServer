using System;
using System.Linq;
using System.Threading;
using NLog;
using Xunit;
using Zidium.Agent.AgentTasks.OutdatedMetrics;
using Zidium.Storage;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Statuses
{
    public class MetricsStatusesTests
    {
        [Fact]
        public void Outdated()
        {
            // получим компонент
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var cpuMetricType = TestHelper.CreateCpuMetricType(account.Id);
            var hddMetricType = TestHelper.CreateHddMetricType(account.Id);

            // данных нет, компонент серый
            TestHelper.CheckExternalStatus(account.Id, component, MonitoringStatus.Unknown);

            // отправим CPU = 50 % = Success
            var response = component.SendMetric(cpuMetricType.SystemName, 50, TimeSpan.FromHours(1));
            Assert.Equal(response.Data.Status, Zidium.Api.MonitoringStatus.Success);
            account.SaveAllCaches();
            TestHelper.CheckExternalStatus(account.Id, component, MonitoringStatus.Success);

            // отправим HDD = 5 Gb = Warning
            response = component.SendMetric(hddMetricType.SystemName, 5, TimeSpan.FromHours(1));
            Assert.Equal(response.Data.Status, Zidium.Api.MonitoringStatus.Warning);
            account.SaveAllCaches();
            TestHelper.CheckExternalStatus(account.Id, component, MonitoringStatus.Warning);

            // отправим CPU = 100 % = Alarm
            response = component.SendMetric(cpuMetricType.SystemName, 100, TimeSpan.FromHours(1));
            Assert.Equal(response.Data.Status, Zidium.Api.MonitoringStatus.Alarm);
            account.SaveAllCaches();
            TestHelper.CheckExternalStatus(account.Id, component, MonitoringStatus.Alarm);

            // отправим CPU = 10 % = Success
            response = component.SendMetric(cpuMetricType.SystemName, 10, TimeSpan.FromHours(1));
            Assert.Equal(response.Data.Status, Zidium.Api.MonitoringStatus.Success);
            account.SaveAllCaches();
            TestHelper.CheckExternalStatus(account.Id, component, MonitoringStatus.Warning);// Warning из-за HDD

            // сделаем так, чтобы метрики протухли
            component.SendMetric(cpuMetricType.SystemName, 10, TimeSpan.FromSeconds(1));
            component.SendMetric(hddMetricType.SystemName, 5, TimeSpan.FromSeconds(1));
            Thread.Sleep(2000);
            account.SaveAllCaches();

            // обновим статус метрик
            var metricProcessor = new OutdatedMetricsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            metricProcessor.ProcessAccount(account.Id);
            account.SaveAllCaches();

            // проверим, что в БД статус протух
            using (var accountDbContext = account.GetAccountDbContext())
            {
                var componentDb = accountDbContext.Components.Find(component.Info.Id);

                var hddMetricDb = componentDb.Metrics.Single(x => x.MetricTypeId == hddMetricType.Id);
                Assert.Null(hddMetricDb.Value);
                Assert.False(hddMetricDb.Bulb.HasSignal);
                Assert.Equal("Нет сигнала", hddMetricDb.Bulb.Message);
                Assert.Equal(hddMetricDb.Bulb.Status, MonitoringStatus.Alarm);

                var cpuMetricDb = componentDb.Metrics.Single(x => x.MetricTypeId == cpuMetricType.Id);
                Assert.Null(cpuMetricDb.Value);
                Assert.False(cpuMetricDb.Bulb.HasSignal);
                Assert.Equal("Нет сигнала", cpuMetricDb.Bulb.Message);
                Assert.Equal(cpuMetricDb.Bulb.Status, MonitoringStatus.Alarm);
            }

            // метрики должны стать красными, т.к. протухли
            TestHelper.CheckMetricaStatus(account.Id, component, cpuMetricType.SystemName, MonitoringStatus.Alarm);
            TestHelper.CheckMetricaStatus(account.Id, component, hddMetricType.SystemName, MonitoringStatus.Alarm);
            TestHelper.CheckExternalStatus(account.Id, component, MonitoringStatus.Alarm);

            // сделаем метрику CPU зеленой
            response = component.SendMetric(cpuMetricType.SystemName, 10, TimeSpan.FromHours(1));
            Assert.Equal(response.Data.Status, Zidium.Api.MonitoringStatus.Success);
            account.SaveAllCaches();
            TestHelper.CheckMetricaStatus(account.Id, component, cpuMetricType.SystemName, MonitoringStatus.Success);
            TestHelper.CheckMetricaStatus(account.Id, component, hddMetricType.SystemName, MonitoringStatus.Alarm);
            TestHelper.CheckExternalStatus(account.Id, component, MonitoringStatus.Alarm);

            // сделаем метрику HDD зеленой
            response = component.SendMetric(hddMetricType.SystemName, 100, TimeSpan.FromHours(1));
            Assert.Equal(response.Data.Status, Zidium.Api.MonitoringStatus.Success);
            account.SaveAllCaches();
            TestHelper.CheckMetricaStatus(account.Id, component, cpuMetricType.SystemName, MonitoringStatus.Success);
            TestHelper.CheckMetricaStatus(account.Id, component, hddMetricType.SystemName, MonitoringStatus.Success);
            TestHelper.CheckExternalStatus(account.Id, component, MonitoringStatus.Success);

            // у каждой колбаски должно быть только 1 актуальное событие
            TestHelper.CheckActualStatusEventsCount(account, component.Info.Id);
        }

        [Fact]
        public void OutdatedGray()
        {
            // создадим метрику без правил
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var metricName = "Metric." + Guid.NewGuid();

            component.SendMetric(metricName, 100, TimeSpan.FromHours(1));

            // все серое, т.к. нет правил метрики
            TestHelper.CheckExternalStatus(account.Id, component, MonitoringStatus.Unknown);
            TestHelper.CheckMetricaStatus(account.Id, component, metricName, MonitoringStatus.Unknown);

            // сделаем так, чтобы метрика протухла
            component.SendMetric(metricName, 100, TimeSpan.FromSeconds(1));
            Thread.Sleep(2000);

            // обновим статус метрики
            var metricProcessor = new OutdatedMetricsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            metricProcessor.ProcessAccount(account.Id);

            // метрика должна стать красной
            TestHelper.CheckMetricaStatus(account.Id, component, metricName, MonitoringStatus.Alarm);
            TestHelper.CheckExternalStatus(account.Id, component, MonitoringStatus.Alarm);

            TestHelper.CheckActualStatusEventsCount(account, component.Info.Id);

            // сделаем метрику серой
            component.SendMetric(metricName, 100, TimeSpan.FromHours(1));
            TestHelper.CheckMetricaStatus(account.Id, component, metricName, MonitoringStatus.Unknown);
            TestHelper.CheckExternalStatus(account.Id, component, MonitoringStatus.Unknown);

            // у каждой колбаски должно быть только 1 актуальное событие
            TestHelper.CheckActualStatusEventsCount(account, component.Info.Id);
        }
    }
}
