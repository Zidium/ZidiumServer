using System;
using Zidium.Api;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.ClientTests
{
    public class ServerTimeTests : BaseTest
    {
        /// <summary>
        /// Тест проверяет, что после вызова client.GetServerTime свойтсво TimeDifferenceSeconds автоматически устанавливается
        /// </summary>
        [Fact]
        public void GetServerTime()
        {
            // проверим, что до 1-го вызова GetServerTime свойство TimeDifferenceSeconds = NULL
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            Assert.Null(client.TimeDifference);

            // вызовем GetServerTime
            var localTime = DateTime.Now;
            var serverDate = client.ToServerTime(localTime);
            Assert.NotNull(client.TimeDifference);

            // проверим, что установка TimeDifferenceSeconds влияет на client.GetServerTime
            client.TimeDifference = TimeSpan.FromSeconds(1);
            serverDate = client.ToServerTime(localTime);
            Assert.NotEqual(localTime, serverDate);
            Assert.Equal(localTime.AddSeconds(1), serverDate);
            
            client.TimeDifference = TimeSpan.Zero;
            serverDate = client.ToServerTime(localTime);
            Assert.Equal(localTime, serverDate);
        }

        /// <summary>
        /// Тест проверяет, что разные часовые пояса не влияют на корректное вычисление разницы времени
        /// </summary>
        [Fact]
        public void TimeZoneTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var root = client.GetRootComponentControl();
            
            var errorData = root
                .CreateApplicationError("timeZoneTest")
                .SetJoinInterval(TimeSpan.Zero);

            ///////////////////
            // ВРЕМЯ НЕ УКАЗАНО
            ///////////////////
            
            // отправим синхронно
            var response = errorData.Send();
            response.Check();

            // отправим асинхронно
            var addResult = errorData.Add();
            client.EventManager.Flush();
            Assert.True(addResult.EventId.HasValue);


            ///////////////////
            // ВРЕМЯ УКАЗАНО
            ///////////////////
            errorData.StartDate = DateTime.Now;
             
            // отправим синхронно
            response = errorData.Send();
            Assert.True(response.Success);

            // отправим асинхронно
            addResult = errorData.Add();
            client.EventManager.Flush();
            Assert.True(addResult.EventId.HasValue);


            /////////////////////////////////////////////////
            // Представим, что на клиенте на 12 часов больше
            /////////////////////////////////////////////////
            var diff = TimeSpan.FromHours(12);
            errorData.StartDate = DateTime.Now + diff;

            // отправим синхронно
            response = errorData.Send();
            Assert.False(response.Success);
            Assert.Equal(ResponseCode.FutureEvent, response.Code);

            // отправим асинхронно
            addResult = errorData.Add();
            client.EventManager.Flush();
            Assert.False(addResult.EventId.HasValue);
        }

        /// <summary>
        /// Тест проверяет, что корректировка времени не влияет на исходные значения StartDate и EndDate события
        /// </summary>
        [Fact]
        public void TimeZoneTest2()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var root = client.GetRootComponentControl();

            var diff = TimeSpan.FromHours(12);
            client.TimeDifference = -diff;
            var startDate = DateTime.Now + diff;
            var endDate = startDate.AddSeconds(1);

            var errorData = root
                .CreateApplicationError("timeZoneTest2")
                .SetJoinInterval(TimeSpan.Zero)
                .SetStartDate(startDate);

            errorData.IsServerTime = false;
            
            // отправим синхронно
            var response = errorData.Send();
            Assert.True(response.Success);
            
            // отправим асинхронно
            errorData = root
                .CreateApplicationError("timeZoneTest2")
                .SetJoinInterval(TimeSpan.Zero)
                .SetStartDate(startDate);

            var addResult = errorData.Add();
            client.EventManager.Flush();
            Assert.True(addResult.EventId.HasValue);
            Assert.Equal(AddEventStatus.Sended, addResult.Status);
            Assert.Equal(startDate, errorData.StartDate);

            // убедимся, что без верного TimeDifferenceSeconds ничего работать не будет
            client.TimeDifference = TimeSpan.Zero;
            
            // отправим синхронно
            errorData = root
                .CreateApplicationError("timeZoneTest2")
                .SetJoinInterval(TimeSpan.Zero)
                .SetStartDate(startDate);

            response = errorData.Send();
            Assert.False(response.Success);
            Assert.Equal(ResponseCode.FutureEvent, response.Code);
            Assert.Equal(startDate, errorData.StartDate);

            // отправим асинхронно
            addResult = errorData.Add();
            client.EventManager.Flush();
            Assert.False(addResult.EventId.HasValue);
            Assert.Equal(AddEventStatus.WaitForSend, addResult.Status);
            Assert.Equal(startDate, errorData.StartDate);
        }
    }
}
