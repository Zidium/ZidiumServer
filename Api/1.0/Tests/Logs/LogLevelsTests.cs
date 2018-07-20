using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Api;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.Logs
{
    
    public class LogLevelsTests
    {
        [Fact]
        public void Test()
        {
            // получим онлайн сервис
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            var messages = new List<string>();
            account.GetClient().WebLogManager.OnAddLogMessage += (control, message) =>
            {
                string tempMessage = message.Level + ":" + message.Message;
                foreach (var property in message.Properties)
                {
                    tempMessage += "; " + property.Name + "=" + property.Value;
                }
                messages.Add(tempMessage);
            };

            // настроим конфиг
            component.Log.WebLogConfig.Enabled = true;
            component.Log.WebLogConfig.IsTraceEnabled = true;
            component.Log.WebLogConfig.IsDebugEnabled = true;
            component.Log.WebLogConfig.IsInfoEnabled = true;
            component.Log.WebLogConfig.IsWarningEnabled = true;
            component.Log.WebLogConfig.IsErrorEnabled = true;
            component.Log.WebLogConfig.IsFatalEnabled = true;

            var parameters = new Dictionary<string, object>()
            {
                {"Name", "Alexey"},
                {"Login", "Smirmov"}
            };
            var paramsObj = new {Id = 100200, DisplayName = "Test display name"};

            // Trace
            component.Log.Trace("text 1");
            component.Log.Trace("text 2", parameters);
            component.Log.Trace("text 3", paramsObj);
            component.Log.TraceFormat("text {0}", 4);
            component.Log.TraceFormat("text {0}", parameters, 5);

            // Debug
            component.Log.Debug("text 1");
            component.Log.Debug("text 2", parameters);
            component.Log.Debug("text 3", paramsObj);
            component.Log.DebugFormat("text {0}", 4);
            component.Log.DebugFormat("text {0}", parameters, 5);

            // Info
            component.Log.Info("text 1");
            component.Log.Info("text 2", parameters);
            component.Log.Info("text 3", paramsObj);
            component.Log.InfoFormat("text {0}", 4);
            component.Log.InfoFormat("text {0}", parameters, 5);

            // Warning
            component.Log.Warning("text 1");
            component.Log.Warning("text 2", parameters);
            component.Log.Warning("text 3", paramsObj);
            component.Log.WarningFormat("text {0}", 4);
            component.Log.WarningFormat("text {0}", parameters, 5);

            // Error
            component.Log.Error("text 1");
            component.Log.Error("text 2", parameters);
            component.Log.Error("text 3", paramsObj);
            component.Log.ErrorFormat("text {0}", 4);
            component.Log.ErrorFormat("text {0}", parameters, 5);

            // Fatal
            component.Log.Fatal("text 1");
            component.Log.Fatal("text 2", parameters);
            component.Log.Fatal("text 3", paramsObj);
            component.Log.FatalFormat("text {0}", 4);
            component.Log.FatalFormat("text {0}", parameters, 5);

            component.Log.Flush();

            var levels = new[]
            {
                LogLevel.Trace, 
                LogLevel.Debug, 
                LogLevel.Info, 
                LogLevel.Warning, 
                LogLevel.Error, 
                LogLevel.Fatal
            };
            foreach (var level in levels)
            {
                Assert.False(messages.Contains(level + ":text 0"));
                Assert.True(messages.Contains(level + ":text 1"));
                Assert.True(messages.Contains(level + ":text 2; Name=Alexey; Login=Smirmov"));
                Assert.True(messages.Contains(level + ":text 3; Id=100200; DisplayName=Test display name"));
                Assert.True(messages.Contains(level + ":text 4"));
                Assert.True(messages.Contains(level + ":text 5; Name=Alexey; Login=Smirmov"));
            }

            // проверим логирование ошибок
            messages.Clear();

            Exception exception;
            try
            {
                throw new Exception("Mega exception");
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Trace
            component.Log.Trace(exception);
            component.Log.Trace("test 1", exception);
            component.Log.Trace("test 2", exception, parameters);
            component.Log.Trace("test 3", exception, paramsObj);

            // Debug
            component.Log.Debug(exception);
            component.Log.Debug("test 1", exception);
            component.Log.Debug("test 2", exception, parameters);
            component.Log.Debug("test 3", exception, paramsObj);

            // Info
            component.Log.Info(exception);
            component.Log.Info("test 1", exception);
            component.Log.Info("test 2", exception, parameters);
            component.Log.Info("test 3", exception, paramsObj);

            // Warning
            component.Log.Warning(exception);
            component.Log.Warning("test 1", exception);
            component.Log.Warning("test 2", exception, parameters);
            component.Log.Warning("test 3", exception, paramsObj);

            // Error
            component.Log.Error(exception);
            component.Log.Error("test 1", exception);
            component.Log.Error("test 2", exception, parameters);
            component.Log.Error("test 3", exception, paramsObj);

            // Fatal
            component.Log.Fatal(exception);
            component.Log.Fatal("test 1", exception);
            component.Log.Fatal("test 2", exception, parameters);
            component.Log.Fatal("test 3", exception, paramsObj);

            component.Log.Flush();

            foreach (var level in levels)
            {
                Assert.False(messages.Contains(level + ":text 0"));
                Assert.True(messages.Any(t => t.StartsWith(level + ":Mega exception;")));
                Assert.True(messages.Any(t => t.StartsWith(level + ":test 1 : Mega exception;")));
                Assert.True(messages.Any(t => t.StartsWith(level + ":test 2 : Mega exception; Name=Alexey; Login=Smirmov;")));
                Assert.True(messages.Any(t => t.StartsWith(level + ":test 3 : Mega exception; Id=100200; DisplayName=Test display name;")));
            }
        }
    }
}
