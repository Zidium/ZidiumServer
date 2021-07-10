using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Zidium.Api.Dto;
using Zidium.TestTools;

namespace Zidium.Api.Tests.Logs
{
    public class AutoCreateEventsTests : BaseTest
    {
        /// <summary>
        /// Тест проверяет, что лог с exception уровня Fatal создаёт красное событие
        /// </summary>
        [Fact]
        public void LogFatalExceptionTest()
        {
            var account = TestHelper.GetTestAccount();
            account.GetClient().Config.Logs.AutoCreateEvents.Disable = false;
            var component = account.CreateRandomComponentControl();

            component.Log.Fatal(new Exception("Test"));
            account.GetClient().Flush();

            var events = account.GetClient().ApiService.GetEvents(new GetEventsRequestDataDto()
            {
                OwnerId = component.Info.Id,
                Category = EventCategory.ApplicationError
            }).GetDataAndCheck();

            Assert.Contains(events, t => t.Importance == EventImportance.Alarm && t.Properties.Any(x => x.Name == "FromLog"));
        }

        /// <summary>
        /// Тест проверяет, что лог с exception уровня Error создаёт жёлтое событие
        /// </summary>
        [Fact]
        public void LogErrorExceptionTest()
        {
            var account = TestHelper.GetTestAccount();
            account.GetClient().Config.Logs.AutoCreateEvents.Disable = false;
            var component = account.CreateRandomComponentControl();

            component.Log.Error(new Exception("Test"));
            account.GetClient().Flush();

            var events = account.GetClient().ApiService.GetEvents(new GetEventsRequestDataDto()
            {
                OwnerId = component.Info.Id,
                Category = EventCategory.ApplicationError
            }).GetDataAndCheck();

            Assert.Contains(events, t => t.Importance == EventImportance.Warning && t.Properties.Any(x => x.Name == "FromLog"));
        }

        /// <summary>
        /// Тест проверяет, что лог с exception уровня Warning создаёт жёлтое событие
        /// </summary>
        [Fact]
        public void LogWarningExceptionTest()
        {
            var account = TestHelper.GetTestAccount();
            account.GetClient().Config.Logs.AutoCreateEvents.Disable = false;
            var component = account.CreateRandomComponentControl();

            component.Log.Warning(new Exception("Test"));
            account.GetClient().Flush();

            var events = account.GetClient().ApiService.GetEvents(new GetEventsRequestDataDto()
            {
                OwnerId = component.Info.Id,
                Category = EventCategory.ApplicationError
            }).GetDataAndCheck();

            Assert.Contains(events, t => t.Importance == EventImportance.Warning && t.Properties.Any(x => x.Name == "FromLog"));
        }

        /// <summary>
        /// Тест проверяет, что лог с exception уровня Info создаёт зелёное событие
        /// </summary>
        [Fact]
        public void LogInfoExceptionTest()
        {
            var account = TestHelper.GetTestAccount();
            account.GetClient().Config.Logs.AutoCreateEvents.Disable = false;
            var component = account.CreateRandomComponentControl();

            component.Log.Info(new Exception("Test"));
            account.GetClient().Flush();

            var events = account.GetClient().ApiService.GetEvents(new GetEventsRequestDataDto()
            {
                OwnerId = component.Info.Id,
                Category = EventCategory.ApplicationError
            }).GetDataAndCheck();

            Assert.Contains(events, t => t.Importance == EventImportance.Success && t.Properties.Any(x => x.Name == "FromLog"));
        }

        /// <summary>
        /// Тест проверяет, что лог с exception уровня Debug создаёт серое событие
        /// </summary>
        [Fact]
        public void LogDebugExceptionTest()
        {
            var account = TestHelper.GetTestAccount();
            account.GetClient().Config.Logs.AutoCreateEvents.Disable = false;
            var component = account.CreateRandomComponentControl();

            component.Log.Debug(new Exception("Test"));
            account.GetClient().Flush();

            var events = account.GetClient().ApiService.GetEvents(new GetEventsRequestDataDto()
            {
                OwnerId = component.Info.Id,
                Category = EventCategory.ApplicationError
            }).GetDataAndCheck();

            Assert.Contains(events, t => t.Importance == EventImportance.Unknown && t.Properties.Any(x => x.Name == "FromLog"));
        }

        /// <summary>
        /// Тест проверяет, что лог с exception уровня Trace создаёт серое событие
        /// </summary>
        [Fact]
        public void LogTraceExceptionTest()
        {
            var account = TestHelper.GetTestAccount();
            account.GetClient().Config.Logs.AutoCreateEvents.Disable = false;
            var component = account.CreateRandomComponentControl();

            component.Log.Trace(new Exception("Test"));
            account.GetClient().Flush();

            var events = account.GetClient().ApiService.GetEvents(new GetEventsRequestDataDto()
            {
                OwnerId = component.Info.Id,
                Category = EventCategory.ApplicationError
            }).GetDataAndCheck();

            Assert.Contains(events, t => t.Importance == EventImportance.Unknown && t.Properties.Any(x => x.Name == "FromLog"));
        }

        /// <summary>
        /// Тест проверяет, что лог без exception уровня Warning создаёт событие
        /// </summary>
        [Fact]
        public void LogWarningMessageOnlyTest()
        {
            var account = TestHelper.GetTestAccount();
            account.GetClient().Config.Logs.AutoCreateEvents.Disable = false;
            var component = account.CreateRandomComponentControl();

            component.Log.Warning("Тестовая ошибка");
            account.GetClient().Flush();

            var events = account.GetClient().ApiService.GetEvents(new GetEventsRequestDataDto()
            {
                OwnerId = component.Info.Id,
                Category = EventCategory.ApplicationError
            }).GetDataAndCheck();

            var _event = events.FirstOrDefault(t => t.Importance == EventImportance.Warning && t.Properties.Any(x => x.Name == "FromLog"));
            Assert.NotNull(_event);
        }

        /// <summary>
        /// Тест проверяет, что лог без exception уровня Info не создаёт событие
        /// </summary>
        [Fact]
        public void LogInfoMessageOnlyTest()
        {
            var account = TestHelper.GetTestAccount();
            account.GetClient().Config.Logs.AutoCreateEvents.Disable = false;
            var component = account.CreateRandomComponentControl();

            component.Log.Info("Тестовое сообщение");
            account.GetClient().Flush();

            var events = account.GetClient().ApiService.GetEvents(new GetEventsRequestDataDto()
            {
                OwnerId = component.Info.Id,
                Category = EventCategory.ApplicationError
            }).GetDataAndCheck();

            var _event = events.FirstOrDefault(t => t.Importance == EventImportance.Success && t.Properties.Any(x => x.Name == "FromLog"));
            Assert.Null(_event);
        }

        /// <summary>
        /// Тест проверяет, что при отключенном автосоздании события не создаются
        /// </summary>
        [Fact]
        public void LogAutocreateDisabledTest()
        {
            var account = TestHelper.GetTestAccount();
            account.GetClient().Config.Logs.AutoCreateEvents.Disable = true;
            var component = account.CreateRandomComponentControl();

            component.Log.Error(new Exception("Test"));
            account.GetClient().Flush();

            var events = account.GetClient().ApiService.GetEvents(new GetEventsRequestDataDto()
            {
                OwnerId = component.Info.Id,
                Category = EventCategory.ApplicationError
            }).GetDataAndCheck();

            Assert.DoesNotContain(events, t => t.Properties.Any(x => x.Name == "FromLog"));
        }

        /// <summary>
        /// Тест проверяет, что свойства из записи лога попадают в событие
        /// </summary>
        [Fact]
        public void LogWithPropertiesTest()
        {
            var account = TestHelper.GetTestAccount();
            account.GetClient().Config.Logs.AutoCreateEvents.Disable = false;
            var component = account.CreateRandomComponentControl();

            var properties = new Dictionary<string, object>(){{"Test property", "Test value"}};
            component.Log.Fatal("Тестовая ошибка", properties);
            account.GetClient().Flush();

            var events = account.GetClient().ApiService.GetEvents(new GetEventsRequestDataDto()
            {
                OwnerId = component.Info.Id,
                Category = EventCategory.ApplicationError
            }).GetDataAndCheck();

            var _event = events.FirstOrDefault(t => t.Importance == EventImportance.Alarm && t.Properties.Any(x => x.Name == "FromLog"));
            Assert.NotNull(_event);
            Assert.Contains(_event.Properties, t => t.Name == "Test property" && t.Value == "Test value");
        }

        /// <summary>
        /// Тест проверяет, какой будет стек у события по записи лога без исключения
        /// </summary>
        [Fact]
        public void LogNoExceptionStackTest()
        {
            var account = TestHelper.GetTestAccount();
            account.GetClient().Config.Logs.AutoCreateEvents.Disable = false;
            var component = account.CreateRandomComponentControl();

            component.Log.Fatal("Тестовая ошибка");
            account.GetClient().Flush();

            var events = account.GetClient().ApiService.GetEvents(new GetEventsRequestDataDto()
            {
                OwnerId = component.Info.Id,
                Category = EventCategory.ApplicationError
            }).GetDataAndCheck();

            var _event = events.FirstOrDefault(t => t.Importance == EventImportance.Alarm && t.Properties.Any(x => x.Name == "FromLog"));
            Assert.NotNull(_event);
            var stack = _event.Properties.FirstOrDefault(t => t.Name == ExtentionPropertyName.Stack && !string.IsNullOrEmpty(t.Value));
            Assert.NotNull(stack);
            Assert.Contains("Zidium.Api.Tests.Logs.AutoCreateEventsTests.LogNoExceptionStackTest()", stack.Value);
        }

        /// <summary>
        /// Тест проверяет, какой будет стек у события по записи лога с исключением
        /// </summary>
        [Fact]
        public void LogExceptionStackTest()
        {
            var account = TestHelper.GetTestAccount();
            account.GetClient().Config.Logs.AutoCreateEvents.Disable = false;
            var component = account.CreateRandomComponentControl();

            Exception exception = null;
            try
            {
                throw new Exception("Test");
            }
            catch (Exception e)
            {
                exception = e;
            }

            component.Log.Fatal(exception);
            account.GetClient().Flush();

            var events = account.GetClient().ApiService.GetEvents(new GetEventsRequestDataDto()
            {
                OwnerId = component.Info.Id,
                Category = EventCategory.ApplicationError
            }).GetDataAndCheck();

            var _event = events.FirstOrDefault(t => t.Importance == EventImportance.Alarm && t.Properties.Any(x => x.Name == "FromLog"));
            Assert.NotNull(_event);
            var stack = _event.Properties.FirstOrDefault(t => t.Name == ExtentionPropertyName.Stack && !string.IsNullOrEmpty(t.Value));
            Assert.NotNull(stack);
            Assert.Contains("Zidium.Api.Tests.Logs.AutoCreateEventsTests.LogExceptionStackTest()", stack.Value);
        }

        /// <summary>
        /// Тест проверяет, какое будет сообщение у события для записи лога с текстом 
        /// </summary>
        [Fact]
        public void LogTextOnlyMessageTest()
        {
            var account = TestHelper.GetTestAccount();
            account.GetClient().Config.Logs.AutoCreateEvents.Disable = false;
            var component = account.CreateRandomComponentControl();

            component.Log.Fatal("Тестовая ошибка");
            account.GetClient().Flush();

            var events = account.GetClient().ApiService.GetEvents(new GetEventsRequestDataDto()
            {
                OwnerId = component.Info.Id,
                Category = EventCategory.ApplicationError
            }).GetDataAndCheck();

            var _event = events.FirstOrDefault(t => t.Importance == EventImportance.Alarm && t.Properties.Any(x => x.Name == "FromLog"));
            Assert.NotNull(_event);
            Assert.Equal("Тестовая ошибка", _event.Message);
        }

        /// <summary>
        /// Тест проверяет, какое будет сообщение у события для записи лога c исключением
        /// </summary>
        [Fact]
        public void LogExceptionOnlyMessageTest()
        {
            var account = TestHelper.GetTestAccount();
            account.GetClient().Config.Logs.AutoCreateEvents.Disable = false;
            var component = account.CreateRandomComponentControl();

            Exception exception;
            try
            {
                throw new Exception("Test error");
            }
            catch (Exception e)
            {
                exception = e;
            }

            component.Log.Fatal(exception);
            account.GetClient().Flush();

            var events = account.GetClient().ApiService.GetEvents(new GetEventsRequestDataDto()
            {
                OwnerId = component.Info.Id,
                Category = EventCategory.ApplicationError
            }).GetDataAndCheck();

            var _event = events.FirstOrDefault(t => t.Importance == EventImportance.Alarm && t.Properties.Any(x => x.Name == "FromLog"));
            Assert.NotNull(_event);
            Assert.Equal("Test error", _event.Message);
        }

        /// <summary>
        /// Тест проверяет, какое будет сообщение у события для записи лога c исключением и текстом
        /// </summary>
        [Fact]
        public void LogTextAndExceptionMessageTest()
        {
            var account = TestHelper.GetTestAccount();
            account.GetClient().Config.Logs.AutoCreateEvents.Disable = false;
            var component = account.CreateRandomComponentControl();

            Exception exception;
            try
            {
                throw new Exception("Test error");
            }
            catch (Exception e)
            {
                exception = e;
            }

            component.Log.Fatal("Тестовая ошибка", exception);
            account.GetClient().Flush();

            var events = account.GetClient().ApiService.GetEvents(new GetEventsRequestDataDto()
            {
                OwnerId = component.Info.Id,
                Category = EventCategory.ApplicationError
            }).GetDataAndCheck();

            var _event = events.FirstOrDefault(t => t.Importance == EventImportance.Alarm && t.Properties.Any(x => x.Name == "FromLog"));
            Assert.NotNull(_event);
            Assert.Equal("Тестовая ошибка : Test error", _event.Message);
        }

    }
}
