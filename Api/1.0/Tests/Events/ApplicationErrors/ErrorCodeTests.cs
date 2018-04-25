using System;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.Events.ApplicationErrors
{
    
    public class ErrorCodeTests
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
                eventId = component.SendApplicationError(exception).Data.EventId;
            }
            var eventObj = component.Client.ApiService.GetEventById(eventId).Data;
            Assert.NotNull(eventObj.TypeCode);
        }
    }
}
