using System;
using System.IO;
using System.Text;
using Zidium.Api;
using Zidium.Api.XmlConfig;
using Xunit;

namespace ApiTests_1._0.Others
{
    public class ConfigTests : BaseTest
    {
        protected string GetConfigPath(string name)
        {
            string appDir = Tools.GetApplicationDir();
            string path = Path.Combine(appDir, "Files\\" + name);
            return path;
        }

        /// <summary>
        /// Проверяем какие настройки будут у клиента, если xml конфиг отсутствует
        /// </summary>
        [Fact]
        public void NoXmlConfigTest()
        {
            var config = new Config();

            // access
            Assert.Equal(config.Access.WaitOnError, TimeSpan.FromMinutes(1));
            Assert.Null(config.Access.Url);

            // все логи выключены, кроме веб-лога
            // это нужно, чтобы без конфига можно было сразу отправлять ошибки и веб-лог.

            // defaultComponent
            Assert.NotNull(config.DefaultComponent);
            Assert.Null(config.DefaultComponent.Id);

            // webLog
            Assert.False(config.Logs.WebLog.Disable);
            Assert.Equal(config.Logs.WebLog.BatchBytes, 100 * 1024); // 100 Kb
            Assert.Equal(config.Logs.WebLog.QueueBytes, 1024 * 1024 * 100); // 100 Мбайт
            Assert.Equal(config.Logs.WebLog.Threads, 3);
            Assert.Equal(config.Logs.WebLog.SendPeriod, TimeSpan.FromSeconds(5));

            // internalLog
            Assert.True(config.Logs.InternalLog.Disable);
            Assert.Equal(false, config.Logs.InternalLog.DeleteOldFileOnStartup);
            Assert.Equal(config.Logs.InternalLog.Encoding, Encoding.UTF8);
            Assert.Equal(@"#appDir\Logs\#appName_ZidiumInternal_#date.txt", config.Logs.InternalLog.FilePath);
            Assert.Equal(config.Logs.InternalLog.MinLevel, LogLevel.Warning);

            // auto create events
            Assert.True(config.Logs.AutoCreateEvents.Disable);

            // event manager
            Assert.Equal(config.Events.EventManager.Disabled, false);
            Assert.Equal(config.Events.EventManager.MaxJoin, 1000);
            Assert.Equal(config.Events.EventManager.MaxSend, 1000);
            Assert.Equal(config.Events.EventManager.QueueBytes, 1024 * 1024 * 100); // 100Mb
            Assert.Equal(TimeSpan.FromSeconds(5), config.Events.EventManager.SendPeriod);
            Assert.Equal(config.Events.EventManager.Threads, 1);

            // componentEvent
            Assert.Equal(config.Events.DefaultValues.ComponentEvent.JoinInterval, TimeSpan.FromMinutes(5));

            // applicationError
            Assert.Equal(config.Events.DefaultValues.ApplicationError.JoinInterval, TimeSpan.FromMinutes(5));
        }

        [Fact]
        public void LoadConfigFromXmlTest()
        {
            string path = GetConfigPath("ConfigIgnoreCase.xml");
            var root = ConfigHelper.Load(path);

            // access
            Assert.Equal(root.Access.AccountName, "test-account-name");
            Assert.Equal(root.Access.SecretKey, "my secret 777");
            Assert.Equal(root.Access.WaitOnError, TimeSpan.FromSeconds(34));
            Assert.Equal("myserver", root.Access.Url);

            // defaultComponent
            Assert.NotNull(root.DefaultComponent);
            Assert.Equal(new Guid("3b1377df-0caf-48c8-baa9-7a61be6b8f22"), root.DefaultComponent.Id);

            // webLog
            Assert.Equal(root.Logs.WebLog.Disable, true);
            Assert.Equal(root.Logs.WebLog.Threads, 30);
            Assert.Equal(root.Logs.WebLog.BatchBytes, 31);
            Assert.Equal(root.Logs.WebLog.SendPeriod, TimeSpan.FromSeconds(32));
            Assert.Equal(root.Logs.WebLog.QueueBytes, 33);

            // internalLog
            Assert.Equal(root.Logs.InternalLog.Disable, false);
            Assert.Equal(root.Logs.InternalLog.Encoding, Encoding.UTF7);
            Assert.Equal(root.Logs.InternalLog.FilePath, @"#appDir\Logs\#date\internal_#date_#hour.txt");
            Assert.Equal(root.Logs.InternalLog.MinLevel, LogLevel.Info);
            Assert.Equal(root.Logs.InternalLog.DeleteOldFileOnStartup, false);

            // auto create events
            Assert.True(root.Logs.AutoCreateEvents.Disable);

            // eventManager
            Assert.True(root.Events.EventManager.Disabled);
            Assert.Equal(root.Events.EventManager.SendPeriod, TimeSpan.FromSeconds(23));
            Assert.Equal(root.Events.EventManager.QueueBytes, 50000001);
            Assert.Equal(root.Events.EventManager.Threads, 7);
            Assert.Equal(root.Events.EventManager.MaxJoin, 1002);
            Assert.Equal(root.Events.EventManager.MaxSend, 1001);

            // componentEvent
            var componentEvent = root.Events.DefaultValues.ComponentEvent;
            Assert.Equal(componentEvent.JoinInterval, TimeSpan.FromSeconds(61));

            // applicationError
            var applicationError = root.Events.DefaultValues.ApplicationError;
            Assert.Equal(applicationError.JoinInterval, TimeSpan.FromSeconds(63));
        }

        [Fact]
        public void DefaultXmlConfigTest()
        {
            string path = GetConfigPath("DefaultConfig.xml");
            var root = ConfigHelper.Load(path);

            // access
            Assert.Equal(root.Access.AccountName, "account-xxx");
            Assert.Equal(root.Access.SecretKey, "key-yyy");
            Assert.Equal(root.Access.WaitOnError, TimeSpan.FromSeconds(60));
            Assert.Null(root.Access.Url);

            // defaultComponent
            Assert.NotNull(root.DefaultComponent);
            Assert.Equal(new Guid("3b1377df-0caf-48c8-baa9-7a61be6b8f22"), root.DefaultComponent.Id);

            // webLog
            Assert.Equal(root.Logs.WebLog.Disable, false);
            Assert.Equal(root.Logs.WebLog.Threads, 3);
            Assert.Equal(root.Logs.WebLog.BatchBytes, 100 * 1024); // 100 Kb
            Assert.Equal(root.Logs.WebLog.SendPeriod, TimeSpan.FromSeconds(5));
            Assert.Equal(root.Logs.WebLog.QueueBytes, 1024 * 1024 * 100); // 100 Мбайт

            // internalLog
            Assert.Equal(root.Logs.InternalLog.Disable, false);
            Assert.Equal(root.Logs.InternalLog.Encoding, Encoding.UTF8);
            Assert.Equal(root.Logs.InternalLog.FilePath, @"#appDir\Logs\Zidium_Internal_#date.txt");
            Assert.Equal(root.Logs.InternalLog.MinLevel, LogLevel.Warning);
            Assert.Equal(root.Logs.InternalLog.DeleteOldFileOnStartup, false);

            // auto create events
            Assert.True(root.Logs.AutoCreateEvents.Disable);

            // event manager
            Assert.Equal(root.Events.EventManager.Disabled, false);
            Assert.Equal(root.Events.EventManager.MaxJoin, 1000);
            Assert.Equal(root.Events.EventManager.MaxSend, 1000);
            Assert.Equal(root.Events.EventManager.QueueBytes, 1024 * 1024 * 100); // 100Mb
            Assert.Equal(TimeSpan.FromSeconds(5), root.Events.EventManager.SendPeriod);
            Assert.Equal(root.Events.EventManager.Threads, 1);

            // componentEvent
            Assert.Equal(root.Events.DefaultValues.ComponentEvent.JoinInterval, TimeSpan.FromMinutes(5));

            // applicationError
            Assert.Equal(root.Events.DefaultValues.ApplicationError.JoinInterval, TimeSpan.FromMinutes(5));
        }

        [Fact]
        public void MiniXmlConfigTest()
        {
            string path = GetConfigPath("MiniConfig.xml");
            var root = ConfigHelper.Load(path);

            // access
            Assert.Equal(root.Access.AccountName, "account-xxx");
            Assert.Equal(root.Access.SecretKey, "key-yyy");
            Assert.Equal(root.Access.WaitOnError, TimeSpan.FromSeconds(60));
            Assert.Null(root.Access.Url);

            // defaultComponent
            Assert.NotNull(root.DefaultComponent);
            Assert.Equal(new Guid("3b1377df-0caf-48c8-baa9-7a61be6b8f22"), root.DefaultComponent.Id);

            // webLog
            Assert.Equal(root.Logs.WebLog.Disable, false);
            Assert.Equal(root.Logs.WebLog.Threads, 3);
            Assert.Equal(root.Logs.WebLog.BatchBytes, 100 * 1024); // 100 Kb
            Assert.Equal(root.Logs.WebLog.SendPeriod, TimeSpan.FromSeconds(5));
            Assert.Equal(root.Logs.WebLog.QueueBytes, 1024 * 1024 * 100); // 100 Мбайт

            // internalLog
            Assert.Equal(root.Logs.InternalLog.Disable, true);
            Assert.Equal(root.Logs.InternalLog.Encoding, Encoding.UTF8);
            Assert.Equal(root.Logs.InternalLog.FilePath, @"#appDir\Logs\#appName_ZidiumInternal_#date.txt");
            Assert.Equal(root.Logs.InternalLog.MinLevel, LogLevel.Warning);
            Assert.Equal(root.Logs.InternalLog.DeleteOldFileOnStartup, false);

            // auto create events
            Assert.True(root.Logs.AutoCreateEvents.Disable);

            // event manager
            Assert.Equal(root.Events.EventManager.Disabled, false);
            Assert.Equal(root.Events.EventManager.MaxJoin, 1000);
            Assert.Equal(root.Events.EventManager.MaxSend, 1000);
            Assert.Equal(root.Events.EventManager.QueueBytes, 1024 * 1024 * 100); // 100Mb
            Assert.Equal(TimeSpan.FromSeconds(5), root.Events.EventManager.SendPeriod);
            Assert.Equal(root.Events.EventManager.Threads, 1);

            // componentEvent
            Assert.Equal(root.Events.DefaultValues.ComponentEvent.JoinInterval, TimeSpan.FromMinutes(5));

            // applicationError
            Assert.Equal(root.Events.DefaultValues.ApplicationError.JoinInterval, TimeSpan.FromMinutes(5));
        }
    }
}
