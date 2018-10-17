using System;
using System.Threading;
using Xunit;
using Zidium.Api;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Dispatcher
{
    public class MetricTypesTests
    {
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
            Assert.Equal(Zidium.Api.MonitoringStatus.Alarm, getMetricResponse.Data.Status);

            // ������� "���� ���� ��� �������" � ���� ������� �� �����
            dispatcher.UpdateMetricType(account.Id, new UpdateMetricTypeRequestData()
            {
                MetricTypeId = metricType.Id,
                SystemName = metricType.SystemName,
                DisplayName = metricType.SystemName,
                NoSignalColor = ObjectColor.Yellow
            }).Check();

            // ��������, ��� ������� ����� �����
            getMetricResponse = component.GetMetric(metricType.SystemName);
            Assert.True(getMetricResponse.Success);
            Assert.NotNull(getMetricResponse.Data);
            Assert.Equal(Zidium.Api.MonitoringStatus.Warning, getMetricResponse.Data.Status);
        }
    }
}
