using System;
using System.Threading;
using Zidium.Api;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.UnitTests
{
    public class UnitTestTests
    {
        /// <summary>
        /// Тест проверяет, что если DisplayName не указан, то он будет равен SystemName
        /// </summary>
        [Fact]
        public void DisplayNameTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();
            var unitTest = component.GetOrCreateUnitTestControl("myInstance");
            Assert.Equal("myInstance", unitTest.Info.DisplayName);
            Assert.Equal("myInstance", unitTest.Info.SystemName);
        }

        /// <summary>
        /// Проверяет, что данные проверок успешно отправляются
        /// </summary>
        [Fact]
        public void SendResultTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            client.Config.Access.WaitOnError = TimeSpan.Zero;
            var component = account.CreateRandomComponentControl();

            // 1 - запросим статус неизвестной проверки
            Guid unitTestId = Guid.NewGuid();
            var checkResponse = client.ApiService.GetUnitTestState(unitTestId);
            Assert.False(checkResponse.Success);

            // 2 - отправляем Success
            var unitTest = component.GetOrCreateUnitTestControl("myInstance");
            unitTestId = unitTest.Info.Id;
            var now = TestHelper.GetNow();
            var message = new SendUnitTestResultData()
            {
                ActualInterval = TimeSpan.FromMinutes(10),
                Message = "Ура проверка, работает!",
                Result = UnitTestResult.Success
            };
            var sendResponse = unitTest.SendResult(message);
            Assert.True(sendResponse.Success);

            // 3 - запросим статус нашей проверки
            checkResponse = client.ApiService.GetUnitTestState(unitTestId);
            Assert.True(checkResponse.Success);
            var status = checkResponse.Data;
            Assert.Equal(status.OwnerId, unitTest.Info.Id);
            Assert.Equal(status.Message, message.Message);
            Assert.Equal(status.Status, MonitoringStatus.Success);

            // 4 - отправляем повтор 
            message.Message = "Изменили сообщение";
            sendResponse = unitTest.SendResult(message);
            Assert.True(sendResponse.Success);
            checkResponse = client.ApiService.GetUnitTestState(unitTestId);
            Assert.True(checkResponse.Success);
            status = checkResponse.Data;
            Assert.Equal(status.OwnerId, unitTest.Info.Id);
            Assert.Equal(status.Message, "Изменили сообщение");// новое
            Assert.Equal(status.Status, MonitoringStatus.Success);
        }

        /// <summary>
        /// Проверим, что многопоточность не приведет к созданию накладывающихся друг на друга событий проверки.
        /// Все события должны быть последовательны.
        /// </summary>
        [Fact]
        public void MultiThreadingTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var unitTest = component.GetOrCreateUnitTestControl("UnitTest." + Guid.NewGuid());
            var tasks = new ThreadTaskQueue(10);
            var random = new Random();
            Response firstError = null;
            for (int i = 0; i < 20; i++)
            {
                tasks.Add(() =>
                {
                    int value = 0;
                    lock (tasks)
                    {
                        value = random.Next(100);
                    }
                    Response response = null;
                    if (value > 50)
                    {
                        response = unitTest.SendResult(UnitTestResult.Success, TimeSpan.FromDays(1));
                    }
                    else
                    {
                        response = unitTest.SendResult(UnitTestResult.Alarm, TimeSpan.FromDays(1));
                    }
                    if (response.Success == false && firstError == null)
                    {
                        firstError = response;
                    }
                });
            }
            tasks.WaitForAllTasksCompleted();

            account.GetClient().Flush();
            account.SaveAllCaches();

            if (firstError != null)
            {
                throw new Exception(firstError.ErrorMessage);
            }

            var actualDate = DateTime.Now.AddMinutes(30);
            var events = account.GetClient().ApiService.GetEvents(new GetEventsData()
            {
                OwnerId = unitTest.Info.Id,
                Category = EventCategory.UnitTestStatus,
                From = actualDate
            }).Data;

            Assert.Equal(1, events.Count);// должно быть только 1 актуальное событие
        }

        /// <summary>
        /// Был баг = при отправке 2-х результатов проверки подряд у новой проверки создавался дубль в событиях (из-за того, что при создании результата не запоминалось его EventId)
        /// </summary>
        [Fact]
        public void BugTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var unitTest = component.GetOrCreateUnitTestControl("UnitTest." + Guid.NewGuid());

            // отправим 2 результата проверки подряд

            // 1-ый результат
            var response1 = unitTest.SendResult(UnitTestResult.Success, TimeSpan.FromDays(1));
            Assert.True(response1.Success);

            // 2-ой результат
            var response2 = unitTest.SendResult(UnitTestResult.Alarm, TimeSpan.FromDays(1));
            Assert.True(response2.Success);

            account.SaveAllCaches();

            // проверим, что нет дублей
            var actualDate = DateTime.Now.AddMinutes(30);
            var events = account.GetClient().ApiService.GetEvents(new GetEventsData()
            {
                OwnerId = unitTest.Info.Id,
                Category = EventCategory.UnitTestResult,
                From = actualDate
            }).Data;

            Assert.Equal(1, events.Count);// должно быть только 1 актуальное событие
        }

        /// <summary>
        /// Зелёный результат после красного должен вернуть зелёный цвет проверке
        /// </summary>
        [Fact]
        public void RedToGreenTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var unitTest = component.GetOrCreateUnitTestControl("UnitTest." + Guid.NewGuid());

            // Отправим красный результат
            var response = unitTest.SendResult(new SendUnitTestResultData()
            {
                Result = UnitTestResult.Alarm,
                ActualInterval = TimeSpan.FromMinutes(5)
            });
            Assert.True(response.Success);

            // Проверим, что проверка стала красной
            var state = unitTest.GetState().Data;
            Assert.Equal(MonitoringStatus.Alarm, state.Status);

            // Проверим, что компонент стал красным
            var state2 = component.GetInternalState(false).Data;
            Assert.Equal(MonitoringStatus.Alarm, state2.Status);

            // Отправим зелёный результат
            response = unitTest.SendResult(new SendUnitTestResultData()
            {
                Result = UnitTestResult.Success,
                ActualInterval = TimeSpan.FromMinutes(5)
            });
            Assert.True(response.Success);

            // Проверим, что проверка стала зелёной
            state = unitTest.GetState().Data;
            Assert.Equal(MonitoringStatus.Success, state.Status);

            // Проверим, что компонент стал зелёным
            state2 = component.GetInternalState(false).Data;
            Assert.Equal(MonitoringStatus.Success, state2.Status);

        }

        /// <summary>
        /// Красный результат после зелёного должен установить красный цвет проверке
        /// </summary>
        [Fact]
        public void GreenToRedTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var unitTest = component.GetOrCreateUnitTestControl(Guid.NewGuid().ToString());

            // Отправим зелёный результат
            var response = unitTest.SendResult(new SendUnitTestResultData()
            {
                Result = UnitTestResult.Success,
                ActualInterval = TimeSpan.FromMinutes(5)
            });
            Assert.True(response.Success);

            // Проверим, что проверка стала зелёной
            var state = unitTest.GetState().Data;
            Assert.Equal(MonitoringStatus.Success, state.Status);

            // Проверим, что компонент стал зелёным
            var state2 = component.GetInternalState(false).Data;
            Assert.Equal(MonitoringStatus.Success, state2.Status);

            // Отправим красный результат
            response = unitTest.SendResult(new SendUnitTestResultData()
            {
                Result = UnitTestResult.Alarm,
                ActualInterval = TimeSpan.FromMinutes(5)
            });
            Assert.True(response.Success);

            // Проверим, что проверка стала красной
            state = unitTest.GetState().Data;
            Assert.Equal(MonitoringStatus.Alarm, state.Status);

            // Проверим, что компонент стал красным
            state2 = component.GetInternalState(false).Data;
            Assert.Equal(MonitoringStatus.Alarm, state2.Status);

        }

        [Fact]
        public void DisableAndEnableTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var unitTest = component.GetOrCreateUnitTestControl(Guid.NewGuid().ToString());

            // Отправим зелёный результат
            var response = unitTest.SendResult(new SendUnitTestResultData()
            {
                Result = UnitTestResult.Success,
                ActualInterval = TimeSpan.FromMinutes(5)
            });
            response.Check();

            // Проверим, что проверка стала зелёной
            var state = unitTest.GetState().Data;
            Assert.Equal(MonitoringStatus.Success, state.Status);

            // Выключим проверку
            unitTest.Disable().Check();

            // Подождём 5 сек
            Thread.Sleep(5000);

            // Включим проверку
            unitTest.Enable().Check();

            // Проверим, что проверка стала серой
            state = unitTest.GetState().Data;
            Assert.Equal(MonitoringStatus.Unknown, state.Status);

            // Отправим зелёный результат
            response = unitTest.SendResult(new SendUnitTestResultData()
            {
                Result = UnitTestResult.Success,
                ActualInterval = TimeSpan.FromMinutes(5)
            });
            Assert.True(response.Success);

            // Проверим, что проверка стала зелёной
            state = unitTest.GetState().Data;
            Assert.Equal(MonitoringStatus.Success, state.Status);
        }

        [Fact]
        public void SendMaxActualIntervalTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var unitTest = component.GetOrCreateUnitTestControl(Guid.NewGuid().ToString());

            var response = unitTest.SendResult(new SendUnitTestResultData()
            {
                Result = UnitTestResult.Success,
                ActualInterval = TimeSpan.MaxValue
            });
            response.Check();

            var state = unitTest.GetState().Data;
            Assert.Equal(2050, state.ActualDate.Year);
        }

        [Fact]
        public void SendResultsTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var unitTest1 = component.GetOrCreateUnitTestControl(Guid.NewGuid().ToString());
            var unitTest2 = component.GetOrCreateUnitTestControl(Guid.NewGuid().ToString());

            var data = new []
            {
                new SendUnitTestResultsData()
                {
                    UnitTestId = unitTest1.Info.Id,
                    Result = UnitTestResult.Success
                },
                new SendUnitTestResultsData()
                {
                    UnitTestId = unitTest2.Info.Id,
                    Result = UnitTestResult.Warning
                }
            };

            account.GetClient().ApiService.SendUnitTestResults(data);

            var status1 = unitTest1.GetState().Data.Status;
            var status2 = unitTest2.GetState().Data.Status;

            Assert.Equal(MonitoringStatus.Success, status1);
            Assert.Equal(MonitoringStatus.Warning, status2);
        }
    }
}
