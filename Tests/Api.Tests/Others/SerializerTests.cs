using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zidium.Api.Dto;
using Xunit;
using Zidium.TestTools;

namespace Zidium.Api.Tests.Others
{
    public class SerializerTests : BaseTest
    {
        [Fact]
        public void JsonTest()
        {
            var request = new SendEventRequestDto();
            request.Token = new AccessTokenDto()
            {
                Program = Guid.NewGuid().ToString(),
                SecretKey = Guid.NewGuid().ToString()
            };
            request.Data = new SendEventRequestDataDto()
            {
                ComponentId = Guid.NewGuid(),
                Category = SendEventCategory.ComponentEvent,
                Count = 3,
                Importance = EventImportance.Alarm,
                Message = "Всем привет, это строка на русском языке",
                StartDate = DateTime.Now,
                TypeCode = "code",
                JoinIntervalSeconds = 200,
                JoinKey = 34324,
                Properties = new List<Dto.ExtentionPropertyDto>(),
                TypeSystemName = "type" + Guid.NewGuid(),
                Version = "1.0"
            };

            TestHelper.InitRandomProperties(request.Data.Properties);

            var serializer = new JsonSerializer();
            var data1 = serializer.GetBytes(request);
            var text = Encoding.UTF8.GetString(data1);
            var request2 = serializer.GetObject(request.GetType(), data1);
            var data2 = serializer.GetBytes(request2);
            TestHelper.AreEqual(data1, data2);
        }

        [Fact]
        public void ParseDoubleInfinityTest()
        {
            var serializer = new JsonSerializer();
            var body = @"{""Data"":[{""ComponentId"":""a962695c-9d36-4f7f-b19f-bf9cb67e67b0"",""Name"":""PrivateMemorySize64"",""Value"":61758558208},{""ComponentId"":""a962695c-9d36-4f7f-b19f-bf9cb67e67b0"",""Name"":""CPU_Usage"",""Value"":Infinity},{""ComponentId"":""a962695c-9d36-4f7f-b19f-bf9cb67e67b0"",""Name"":""Disk_C_FreeSpace_Gb"",""Value"":823.77}],""Token"":{""SecretKey"":""ae69c672-3633-475b-9bec-a69d51ac0d27"",""Program"":""ForecastAgent.exe""}}";
            var package = serializer.GetObject<SendMetricsRequestDto>(body);
            Assert.NotNull(package);
            var metricData = package.Data.FirstOrDefault(t => t.Name == "CPU_Usage");
            Assert.NotNull(metricData);
            Assert.True(double.IsInfinity(metricData.Value ?? 0.0));
        }

        [Fact]
        public void ParseDoubleNaNTest()
        {
            var serializer = new JsonSerializer();
            var body = @"{""Data"":[{""ComponentId"":""a962695c-9d36-4f7f-b19f-bf9cb67e67b0"",""Name"":""PrivateMemorySize64"",""Value"":61758558208},{""ComponentId"":""a962695c-9d36-4f7f-b19f-bf9cb67e67b0"",""Name"":""CPU_Usage"",""Value"":NaN},{""ComponentId"":""a962695c-9d36-4f7f-b19f-bf9cb67e67b0"",""Name"":""Disk_C_FreeSpace_Gb"",""Value"":823.77}],""Token"":{""SecretKey"":""ae69c672-3633-475b-9bec-a69d51ac0d27"",""Program"":""ForecastAgent.exe""}}";
            var package = serializer.GetObject<SendMetricsRequestDto>(body);
            Assert.NotNull(package);
            var metricData = package.Data.FirstOrDefault(t => t.Name == "CPU_Usage");
            Assert.NotNull(metricData);
            Assert.True(double.IsNaN(metricData.Value ?? 0.0));
        }
    }
}
