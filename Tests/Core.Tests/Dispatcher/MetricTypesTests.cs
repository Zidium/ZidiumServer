using System;
using System.Threading;
using Xunit;
using Zidium.Api;
using Zidium.Core.Api;
using Zidium.Storage;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Dispatcher
{
    public class MetricTypesTests : BaseTest
    {
        [Fact]
        public void ChangeNoSignalColorTest()
        {
            var account = TestHelper.GetTestAccount();
            var metricType = TestHelper.CreateTestMetricType(account.Id);
            var component = account.CreateRandomComponentControl();
            var dispatcher = TestHelper.GetDispatcherClient();

            // Отправим метрику с актуальностью 1 секунду
            var responseSend = component.SendMetric(
                new SendMetricData()
                {
                    Name = metricType.SystemName,
                    Value = 1000,
                    ActualInterval = TimeSpan.FromSeconds(1)
                });
            responseSend.Check();

            // Подождём 2 секунды
            Thread.Sleep(2 * 1000);

            // Проверим, что метрика красная
            var getMetricResponse = component.GetMetric(metricType.SystemName);
            Assert.True(getMetricResponse.Success);
            Assert.NotNull(getMetricResponse.Data);
            Assert.Equal(Zidium.Api.MonitoringStatus.Alarm, getMetricResponse.Data.Status);

            // Изменим "Цвет если нет сигнала" у типа метрики на жёлтый
            dispatcher.UpdateMetricType(account.Id, new UpdateMetricTypeRequestData()
            {
                MetricTypeId = metricType.Id,
                SystemName = metricType.SystemName,
                DisplayName = metricType.SystemName,
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
