using System;
using Xunit;
using Zidium.Api;
using Zidium.Api.Dto;

namespace Zidium.Core.Tests.Others
{
    public class JsonTests
    {
        [Fact]
        public void UnsuccessResponseTest()
        {
            var response = new Core.Api.SendUnitTestResultResponse()
            {
                Code = ResponseCode.UnknownUnitTestId,
                ErrorMessage = "Неизвестный UnitTestId",
                InternalData = null
            };
            var serializer = new JsonSerializer();
            var responseBytes = serializer.GetBytes(response);
            Assert.NotNull(responseBytes);
        }

        [Fact]
        public void NullEnumValueTest()
        {
            var json = @"{
""Token"": {
""SecretKey"": ""9c7f86b4-69d5-4279-a521-5633a7b276c6""
},
""Data"": {
""ComponentId"": ""f5122540-e209-4d4f-a886-b114637e9119"",
""Importance"": [
""""
]
}
}";
            var serializer = new JsonSerializer();
            Assert.ThrowsAny<ArgumentNullException>(() =>
            {
                var request = serializer.GetObject<Core.Api.GetEventsRequest>(json);
            });
        }
    }
}
