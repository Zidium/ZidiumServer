using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Zidium.Api;
using Zidium.TestTools;

namespace ApiTests_1._0.Metrics
{
    public class MetricsTests
    {
        [Fact]
        public void GetMetricsHistory()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var counterName = Guid.NewGuid().ToString();
            var counterValue = new Random().Next();
            var counterBeginDate = DateTime.Now;
            //var counterActualDate = counterBeginDate.AddDays(1);

            var sendResponse = component.SendMetric(counterName, counterValue, TimeSpan.FromDays(1));
            Assert.True(sendResponse.Success);
            //Assert.NotEqual(Guid.Empty, sendResponse.Data.Value);

            var getResponse = component.GetMetricsHistory(new GetMetricsHistoryFilter()
            {
                From = counterBeginDate.AddMinutes(-5)
            });
            Assert.True(getResponse.Success);

            var counter = getResponse.Data.FirstOrDefault(t => t.Name == counterName);
            Assert.NotNull(counter);
            Assert.Equal(counterValue, counter.Value);
            //Assert.True(TestHelper.CheckDateTimesEqualBySeconds(counterBeginDate, counter.BeginDate));
            //Assert.True(TestHelper.CheckDateTimesEqualBySeconds(counterActualDate, counter.ActualDate));
        }

        [Fact]
        public void GetMetrics()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var counterName = Guid.NewGuid().ToString();

            var getResponse = component.GetMetrics();
            Assert.True(getResponse.Success);
            Assert.Equal(0, getResponse.Data.Count);

            var counterValue = new Random().Next();
            //var counterBeginDate = DateTime.Now;
            //var counterActualDate = counterBeginDate.AddDays(1);

            var sendResponse = component.SendMetric(counterName, counterValue, TimeSpan.FromDays(1));
            sendResponse.Check();

            getResponse = component.GetMetrics();
            Assert.True(getResponse.Success);

            var counter = getResponse.Data[0];
            Assert.NotNull(counter);
            Assert.Equal(counterValue, counter.Value);
            //Assert.True(TestHelper.CheckDateTimesEqualBySeconds(counterBeginDate, counter.BeginDate));
            //Assert.True(TestHelper.CheckDateTimesEqualBySeconds(counterActualDate, counter.ActualDate));
        }

        [Fact]
        public void SendInfinityValueTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var metricName = Guid.NewGuid().ToString();
            var metricValue = double.PositiveInfinity;

            var sendResponse = component.SendMetric(metricName, metricValue);
            Assert.False(sendResponse.Success);
            Assert.Equal(1141, sendResponse.Code);
            Assert.Equal("Metric value can't be Nan or Infinity", sendResponse.ErrorMessage);
        }

        [Fact]
        public void SendInfinityValue2Test()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var metricName = Guid.NewGuid().ToString();
            var metricValue = double.PositiveInfinity;

            var data = new SendMetricData[]
            {
                new SendMetricData()
                {
                    Name = metricName,
                    Value = metricValue
                }
            };

            var sendResponse = component.SendMetrics(data.ToList());
            Assert.False(sendResponse.Success);
            Assert.Equal(1141, sendResponse.Code);
            Assert.Equal("Metric value can't be Nan or Infinity", sendResponse.ErrorMessage);
        }

        [Fact]
        public void SendNaNValueTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var metricName = Guid.NewGuid().ToString();
            var metricValue = double.NaN;

            var sendResponse = component.SendMetric(metricName, metricValue);
            Assert.False(sendResponse.Success);
            Assert.Equal(1141, sendResponse.Code);
            Assert.Equal("Metric value can't be Nan or Infinity", sendResponse.ErrorMessage);
        }

        [Fact]
        public void SendNaNValue2Test()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var metricName = Guid.NewGuid().ToString();
            var metricValue = double.NaN;

            var data = new SendMetricData[]
            {
                new SendMetricData()
                {
                    Name = metricName,
                    Value = metricValue
                }
            };

            var sendResponse = component.SendMetrics(data.ToList());
            Assert.False(sendResponse.Success);
            Assert.Equal(1141, sendResponse.Code);
            Assert.Equal("Metric value can't be Nan or Infinity", sendResponse.ErrorMessage);
        }

        [Fact]
        public void SendMaxActualDateTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var metricName = Guid.NewGuid().ToString();
            var metricValue = 10;

            var sendResponse = component.SendMetric(metricName, metricValue, TimeSpan.MaxValue);
            Assert.True(sendResponse.Success);

            var metricInfo = sendResponse.Data;
            Assert.Equal(2050, metricInfo.ActualDate.Year);
        }
    }
}
