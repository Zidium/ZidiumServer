﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core.Common;
using Zidium.TestTools;

namespace Zidium.Api.Tests.Logs
{
    public class LogTests : BaseTest
    {
        [Fact]
        public void FixText()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var mes = @"GET http://educationadvisor.ru/communication/messages/forum8/topic48/message74/#message74";
            component.Log.Debug(mes);
        }

        [Fact]
        public void SendMessageTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            Assert.False(component.IsFake());

            // настроим конфиг
            component.Log.WebLogConfig.Enabled = true;
            component.Log.WebLogConfig.IsTraceEnabled = true;
            component.Log.WebLogConfig.IsDebugEnabled = true;
            component.Log.WebLogConfig.IsInfoEnabled = true;
            component.Log.WebLogConfig.IsWarningEnabled = true;
            component.Log.WebLogConfig.IsErrorEnabled = true;
            component.Log.WebLogConfig.IsFatalEnabled = true;

            var date = TestHelper.GetServerDateTime();

            component.Log.Debug("Debug 1");
            component.Log.Debug("Debug 2", new { text = "text 2", date });
            component.Log.Debug("Debug 3", new Dictionary<string, object>()
            {
                {"text", "text 3"},
                {"int32", 70000},
            });

            component.Log.Trace("Trace 1");
            component.Log.Trace("Trace 2", new { text = "text 2", date });
            component.Log.Trace("Trace 3", new Dictionary<string, object>()
            {
                {"text", "text 3"},
                {"int32", 70000},
            });

            component.Log.Info("Info 1");
            component.Log.Info("Info 2", new { text = "text 2", date });
            component.Log.Info("Info 3", new Dictionary<string, object>()
            {
                {"text", "text 3"},
                {"int32", 70000},
            });

            component.Log.Warning("Warning 1");
            component.Log.Warning("Warning 2", new { text = "text 2", date });
            component.Log.Warning("Warning 3", new Dictionary<string, object>()
            {
                {"text", "text 3"},
                {"int32", 70000},
            });

            component.Log.Error("Error 1");
            component.Log.Error("Error 2", new { text = "text 2", date });
            component.Log.Error("Error 3", new Dictionary<string, object>()
            {
                {"text", "text 3"},
                {"int32", 70000},
            });

            component.Log.Fatal("Fatal 1");
            component.Log.Fatal("Fatal 2", new { text = "text 2", date });
            component.Log.Fatal("Fatal 3", new Dictionary<string, object>()
            {
                {"text", "text 3"},
                {"int32", 70000},
            });
            component.Log.Flush();

            var endDate = TestHelper.GetServerDateTime().AddSeconds(2);

            // проверим, что все отправленное записалось в БД
            var findMessage = new GetLogsFilter()
            {
                From = date,
                To = endDate,
                Levels = null,
                MaxCount = 100
            };
            var findLogsResponse = component.GetLogs(findMessage);
            Assert.True(findLogsResponse.Success);
            var rows = findLogsResponse.GetDataAndCheck();
            var levels = new[] { "Debug", "Trace", "Info", "Warning", "Error", "Fatal" };
            Assert.Equal(levels.Length * 3, rows.Count);
            foreach (var level in levels)
            {
                var message = rows.First(x => x.Message == level + " " + 1);
                Assert.Null(message.Properties);

                message = rows.First(x => x.Message == level + " " + 2);
                var properties = new ExtentionPropertyCollection(message.Properties);
                Assert.Equal(2, properties.Count);
                string text = properties["text"];
                Assert.Equal("text 2", text);
                DateTime testDate = properties["date"];
                Assert.Equal(date, testDate);

                message = rows.First(x => x.Message == level + " " + 3);
                properties = new ExtentionPropertyCollection(message.Properties);
                Assert.Equal(2, properties.Count);
                text = properties["text"];
                Assert.Equal("text 3", text);
                int int32 = properties["int32"];
                Assert.Equal(70000, int32);
            }
        }

        /// <summary>
        /// Тест проверяет, что Context записывается в БД
        /// </summary>
        [Fact]
        public void ContextTest()
        {
            // запись без тега
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var filter = new GetLogsFilter();
            component.Log.Info("no tag message");
            component.Log.Flush();
            var rows = component.GetLogs(filter).GetDataAndCheck();
            Assert.Single(rows);
            filter.Context = "myTag";
            rows = component.GetLogs(filter).GetDataAndCheck();
            Assert.Empty(rows);

            //запись с тегом
            component.Log.Context = "myTag";
            component.Log.Info("has tag message");
            component.Log.Flush();
            rows = component.GetLogs(filter).GetDataAndCheck();
            Assert.Single(rows);
        }

        [Fact]
        public void OfflineTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            client.Config.Access.WaitOnError = TimeSpan.Zero;
            var onlineService = client.ApiService;

            // получим онлайн контрол
            var component1 = account.CreateRandomComponentControl();
            Assert.False(component1.IsFake());

            // выключим интернет
            var offlineService = new FakeApiService();
            client.SetApiService(offlineService);

            // получим фейковый контрол
            var component2 = account.CreateRandomComponentControl();
            Assert.True(component2.IsFake());

            // отправим 2 сообщения
            component1.Log.Info("test 1");
            component2.Log.Info("test 1");
            component1.Log.Info("test 2");
            component2.Log.Info("test 2");
            component1.Log.Flush();

            // включим интернет
            client.SetApiService(onlineService);

            // проверим, что логов у component1 еще нет
            var logsResponce1 = component1.GetLogs(new GetLogsFilter());
            Assert.True(logsResponce1.Success);
            Assert.Empty(logsResponce1.GetDataAndCheck());

            // проверим, что логов у component2 еще нет
            var logsResponce2 = component2.GetLogs(new GetLogsFilter());
            Assert.True(logsResponce2.Success);
            Assert.Empty(logsResponce2.GetDataAndCheck());

            // получим логи, записанные в оффлайн режиме
            component1.Log.Flush();
            logsResponce1 = component1.GetLogs(new GetLogsFilter());
            Assert.True(logsResponce1.Success);
            Assert.Equal(2, logsResponce1.GetDataAndCheck().Count);

            // у component2 логов не будет, т.к. в оффлайн режиме у него веб-лог конфиг выключен!
            component1.Log.Flush();
            logsResponce2 = component2.GetLogs(new GetLogsFilter());
            Assert.True(logsResponce2.Success);
            Assert.Empty(logsResponce2.GetDataAndCheck());
        }

        [Fact]
        public void ReloadConfigTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var serviceWrapper = (ApiServiceWrapper)client.ApiService;
            var allResponses = new List<ResponseDto>();
            DateTime lastResponseTime = DateTime.MinValue;
            serviceWrapper.ProcessResponseAction = response =>
            {
                allResponses.Add(response);
                lastResponseTime = DateTime.Now;
            };

            // получим компонент
            var component = account.CreateRandomComponentControl();
            Assert.False(component.IsFake());

            // по умолчанию включены все уровни логов
            Assert.True(component.WebLogConfig.IsInfoEnabled);
            Assert.True(component.WebLogConfig.IsWarningEnabled);
            Assert.True(component.Log.IsInfoEnabled);
            Assert.True(component.Log.IsWarningEnabled);
            component.Log.Info("info");
            component.Log.Warning("warning");
            component.Log.Flush();
            var logs = component.GetLogs(new GetLogsFilter()).GetDataAndCheck();
            Assert.Equal(2, logs.Count);

            // будет делать попытку загрузить новые конфиги каждую секунду
            client.Config.Logs.WebLog.ReloadConfigsPeriod = TimeSpan.FromSeconds(1);

            // в личном кабинете выключим info
            account.SetComponentLogConfigIsInfoEnabled(component.Info.Id, false);

            // подождем перезагрузки конфига
            TestHelper.WaitTrue(() => component.WebLogConfig.IsInfoEnabled == false, TimeSpan.FromSeconds(10));

            // проверим, что на клиенте веб-конфиг обновился
            Assert.False(component.WebLogConfig.IsInfoEnabled);
            Assert.True(component.WebLogConfig.IsWarningEnabled);
            Assert.False(component.Log.IsInfoEnabled);
            Assert.True(component.Log.IsWarningEnabled);
            component.Log.Info("info");
            component.Log.Warning("warning");
            component.Log.Flush();
            logs = component.GetLogs(new GetLogsFilter()).GetDataAndCheck();
            Assert.Equal(3, logs.Count);

            // подождем повторной перезагрузки конфигов
            var time = DateTime.Now;
            var configResponses = allResponses;
            TestHelper.WaitTrue(
                () =>
                {
                    if (lastResponseTime < time)
                    {
                        return false;
                    }
                    configResponses = allResponses.Where(x => x.GetType() == typeof(GetChangedWebLogConfigsResponseDto)).ToList();
                    return configResponses.Count > 1;
                },
                TimeSpan.FromSeconds(10));

            // проверим, что последний запрос конфигов был пустой
            var lastConfigResponse = (GetChangedWebLogConfigsResponseDto)configResponses.Last();
            Assert.Empty(lastConfigResponse.GetDataAndCheck());
        }

        [Fact]
        public void BigMessageTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            // отправляем
            var message = RandomHelper.GetRandomStringAZ(9 * 1024, 9 * 1024); // 9 КБ
            var bytes = RandomHelper.GetRandomBytes(1024 * 1024 * 5); // 5 МБ
            component.Log.Info(message, new { myBytes = bytes });
            component.Log.Flush();

            // проверяем
            var logs = component.GetLogs(new GetLogsFilter()).GetDataAndCheck();
            Assert.Single(logs);
            var log = logs.First();
            Assert.Equal(4000, log.Message.Length);
            Assert.Equal(log.Message, message.Substring(0, 4000)); // записываются только первые 8000 символов
            var properties = new ExtentionPropertyCollection(log.Properties);
            var bytes2 = properties["myBytes"].Value as byte[];
            Assert.Equal(bytes, bytes2);
        }

        [Fact]
        public void ApplicationErrorTest()
        {
            // получим онлайн сервис
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            LogMessage logMessage = null;
            account.GetClient().WebLogManager.OnAddLogMessage += (control, message) => logMessage = message;

            ApplicationErrorException exception;
            try
            {
                throw new ApplicationErrorException("testError", "test error");
            }
            catch (ApplicationErrorException e)
            {
                exception = e;
            }

            exception.Properties.Set("bytes", new byte[] { 1, 2, 3 });
            exception.Properties.Set("string", "fff");
            exception.Properties.Set("guid", Ulid.NewUlid());

            // отключим ненужные логи
            component.Log.WebLogConfig.Enabled = true;

            // запишем 2 сообщения
            component.Log.Error("message text", exception);
            component.Log.Flush();

            // проверим в кастомном логе
            Assert.NotNull(logMessage);
            Assert.Equal("message text : test error", logMessage.Message);
            exception.Properties.Set("Stack", logMessage.Properties["Stack"].Value as string);
            TestHelper.CheckExtentionProperties(exception.Properties, logMessage.Properties);

            // проверим в веб-логе
            var logs = component.GetLogs(new GetLogsFilter()).GetDataAndCheck();
            Assert.Single(logs);
            var firstWebLog = logs.First();
            TestHelper.CheckExtentionProperties(exception.Properties, firstWebLog.Properties);
        }


        private void CheckExtProperties(Dictionary<string, object> from, ExtentionPropertyCollection to)
        {
            foreach (var pair in from)
            {
                var key = pair.Key;
                var value = pair.Value;
                if (to.HasKey(key) == false)
                {
                    throw new Exception("Не удалось найти ключ " + key);
                }
                var value2 = to[key].Value;
                Assert.Equal(value, value2);
            }
        }

        [Fact]
        public void ExceptionTest()
        {
            // получим онлайн сервис
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            LogMessage logMessage = null;
            account.GetClient().WebLogManager.OnAddLogMessage += (control, message) => logMessage = message;

            Exception exception;
            try
            {
                throw new Exception("test error");
            }
            catch (Exception e)
            {
                exception = e;
            }

            var exrProperties = new Dictionary<string, object>();
            exrProperties.Add("bytes", new byte[] { 1, 2, 3 });
            exrProperties.Add("string", "fff");
            exrProperties.Add("guid", Ulid.NewUlid());

            foreach (var exrProperty in exrProperties)
            {
                exception.Data.Add(exrProperty.Key, exrProperty.Value);
            }

            // отключим ненужные логи
            component.Log.WebLogConfig.Enabled = true;

            // запишем 2 сообщения
            component.Log.Error("message text", exception);
            component.Log.Flush();

            // проверим в списке
            Assert.NotNull(logMessage);
            Assert.Equal("message text : test error", logMessage.Message);
            var stack = logMessage.Properties["Stack"];
            Assert.NotNull(stack);
            CheckExtProperties(exrProperties, logMessage.Properties);

            // проверим в веб-логе
            var logs = component.GetLogs(new GetLogsFilter()).GetDataAndCheck();
            Assert.Single(logs);
            var firstWebLog = logs.First();
            var properties = new ExtentionPropertyCollection(firstWebLog.Properties);
            CheckExtProperties(exrProperties, properties);
        }

        [Fact]
        public void NullPropertyTest()
        {
            // получим онлайн сервис
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            var messages = new List<string>();
            account.GetClient().WebLogManager.OnAddLogMessage += (control, message) => messages.Add(message.Message);

            // отключим ненужные логи
            component.Log.WebLogConfig.Enabled = true;

            // запишем 2 сообщения
            component.Log.Info("1");
            string accountId = null;
            string version = null;
            component.Log.Error("Ошибка", new { accountId, version });
            component.Log.Flush();

            Assert.Equal(2, messages.Count);
        }

        [Fact]
        public void MessageInMessageTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var mes = @"GET http://educationadvisor.ru/communication/messages/forum8/topic48/message74/#message74";
            component.Log.Debug(mes);
        }

        [Fact]
        public void LogOrderTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var date = TestHelper.GetServerDateTime();

            // Отправим две записи на одну дату
            component.Log.Info("Message1");
            component.Log.Info("Message2");
            component.Log.Flush();

            // Проверим, что вторая запись имеет больший Order, чем первая
            var response = component.GetLogs(new GetLogsFilter()
            {
                From = date
            });
            response.Check();

            var row1 = response.GetDataAndCheck().FirstOrDefault(t => t.Message == "Message1");
            Assert.NotNull(row1);

            var row2 = response.GetDataAndCheck().FirstOrDefault(t => t.Message == "Message2");
            Assert.NotNull(row2);

            Assert.True(row2.Order > row1.Order);
        }

        [Fact]
        public void GetLogsByMessageTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            Assert.False(component.IsFake());

            component.Log.Info("Message 1");
            component.Log.Info("Message 2");
            component.Log.Info("Message 3");
            component.Log.Flush();

            var logs = component.GetLogs(new GetLogsFilter() { Message = "Message 2" }).GetDataAndCheck();

            Assert.Single(logs);
            var log = logs[0];
            Assert.Equal("Message 2", log.Message);
        }
    }
}

