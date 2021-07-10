using System;
using Xunit;
using Zidium.TestTools;

namespace Zidium.Api.Tests.Events.ApplicationErrors
{
    public class ErrorCodeTests : BaseTest
    {
        [Fact]
        public void Test()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            Guid eventId;
            try
            {
                throw new Exception("Тестовая ошибка");
            }
            catch (Exception exception)
            {
                eventId = component.SendApplicationError(exception).GetDataAndCheck().EventId;
            }
            var eventObj = component.Client.ApiService.GetEventById(eventId).GetDataAndCheck();
            Assert.NotNull(eventObj.TypeCode);
        }
    }
}
