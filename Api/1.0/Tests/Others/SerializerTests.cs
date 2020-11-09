using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zidium.Api;
using Zidium.Api.Common;
using Zidium.Api.Dto;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.Others
{
    public class SerializerTests : BaseTest
    {
        protected List<ISerializer> GetSerializers()
        {
            return new List<ISerializer>()
            {
                new JsonSerializer(),
                new XmlSerializer()
            };
        }

        [Fact]
        public void XmlDeflateSerializerTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var root = client.GetRootComponentControl();
            ISerializer xmlSerializer = new XmlSerializer();
            ISerializer deflateSerializer = new XmlDeflateSerializer();

            var mes1 = TestHelper.CreateRandomComponentEvent(root);
            var eventDate = DataConverter.GetSendEventData(mes1);
            byte[] dataXml = xmlSerializer.GetBytes(eventDate);
            byte[] datadeflate = deflateSerializer.GetBytes(eventDate);
            var k = ((float)dataXml.Length)/datadeflate.Length;
            Assert.True(k > 1.6f);

            eventDate = new SendEventRequestDtoData()
            {
                Category = SendEventCategory.ApplicationError,
                StartDate = DateTime.Now,
                Importance = EventImportance.Alarm,
                JoinIntervalSeconds = TimeSpan.FromHours(1).TotalSeconds,
                JoinKey = 45453453,
                Message = "Это короткое тестовое сообщение",
                TypeDisplayName = "Отображаемое имя типа ошибки",
                TypeSystemName = "системное имя ошибки",
                Properties = new List<ExtentionPropertyDto>()
            };
            eventDate.Properties.Add(new ExtentionPropertyDto()
            {
                Name = "int",
                Type = "Int32",
                Value = "3134"
            });
            eventDate.Properties.Add(new ExtentionPropertyDto()
            {
                Name = "string",
                Type = "String",
                Value = "3jdklasjd lkas kasjd klaj dlkajs dlk j134"
            });

            dataXml = xmlSerializer.GetBytes(eventDate);
            datadeflate = deflateSerializer.GetBytes(eventDate);
            var k2 = ((float)dataXml.Length) / datadeflate.Length;
            Assert.True(k2 > 1.0f);
        }

        [Fact]
        public void JsonTest()
        {
            var serializers = GetSerializers();
            foreach (var serializer in serializers)
            {
                var request = new SendEventRequestDto();
                request.Token = new AccessTokenDto()
                {
                    AccountId = Guid.NewGuid(),
                    Program = Guid.NewGuid().ToString(),
                    SecretKey = Guid.NewGuid().ToString()
                };
                request.Data = new SendEventRequestDtoData()
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
                    Properties = new List<ExtentionPropertyDto>(),
                    TypeSystemName = "type" + Guid.NewGuid(),
                    Version = "1.0"
                };
                TestHelper.InitRandomProperties(request.Data.Properties);
                var data1 = serializer.GetBytes(request);
                var text = Encoding.UTF8.GetString(data1);
                var request2 = serializer.GetObject(request.GetType(), data1);
                var data2 = serializer.GetBytes(request2);
                TestHelper.AreEqual(data1, data2);
            }
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
