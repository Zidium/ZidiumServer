using System;
using Xunit;
using Zidium.Api;
using Zidium.TestTools;

namespace ApiTests_1._0.Events.ApplicationErrors
{
    public class SendTests : BaseTest
    {
        /// <summary>
        /// Проверим, что ошибка без поля "стек" успешно сохранится
        /// </summary>
        [Fact]
        public void NullStack()
        {
            var component = TestHelper.GetTestAccount().CreateRandomComponentControl();
            var response = component.CreateApplicationError("type", "message").Send();
            Assert.True(response.Success);
        }

        [Fact]
        public void SendException()
        {
            var exceptionMessage = "Test exception " + DateTime.Now.Ticks;
            try
            {
                throw new Exception(exceptionMessage);
            }
            catch (Exception e)
            {
                var account = TestHelper.GetTestAccount();
                var component = account.CreateRandomComponentControl();

                var response = component.SendApplicationError("моя любимая ошибка", e);
                Assert.True(response.Success);

                var expectedHash = new ExceptionRender().GetExceptionTypeCode(e);

                var eventObj = component.Client.ApiService.GetEventById(response.Data.EventId);
                Assert.Equal(
                    "моя любимая ошибка: Exception в SendTests.SendException() (hash " + expectedHash + ")",
                    eventObj.Data.TypeSystemName);
            }
        }
    }
}
