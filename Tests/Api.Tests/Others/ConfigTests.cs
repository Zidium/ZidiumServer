using System;
using System.IO;
using System.Text;
using Zidium.Api.XmlConfig;
using Xunit;
using Zidium.Api.Dto;

namespace Zidium.Api.Tests.Others
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
            Assert.Equal(3, config.Logs.WebLog.Threads);
            Assert.Equal(config.Logs.WebLog.SendPeriod, TimeSpan.FromSeconds(5));

            // internalLog
            Assert.True(config.Logs.InternalLog.Disable);
            Assert.False(config.Logs.InternalLog.DeleteOldFileOnStartup);
            Assert.Equal(config.Logs.InternalLog.Encoding, Encoding.UTF8);
            Assert.Equal(@"#appDir\Logs\#appName_ZidiumInternal_#date.txt", config.Logs.InternalLog.FilePath);
            Assert.Equal(LogLevel.Warning, config.Logs.InternalLog.MinLevel);

            // auto create events
            Assert.True(config.Logs.AutoCreateEvents.Disable);

            // event manager
            Assert.False(config.Events.EventManager.Disabled);
            Assert.Equal(1000, config.Events.EventManager.MaxJoin);
            Assert.Equal(1000, config.Events.EventManager.MaxSend);
            Assert.Equal(config.Events.EventManager.QueueBytes, 1024 * 1024 * 100); // 100Mb
            Assert.Equal(TimeSpan.FromSeconds(5), config.Events.EventManager.SendPeriod);
            Assert.Equal(1, config.Events.EventManager.Threads);

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
            Assert.Equal("my secret 777", root.Access.SecretKey);
            Assert.Equal(root.Access.WaitOnError, TimeSpan.FromSeconds(34));
            Assert.Equal("myserver", root.Access.Url);

            // defaultComponent
            Assert.NotNull(root.DefaultComponent);
            Assert.Equal(new Guid("3b1377df-0caf-48c8-baa9-7a61be6b8f22"), root.DefaultComponent.Id);

            // webLog
            Assert.True(root.Logs.WebLog.Disable);
            Assert.Equal(30, root.Logs.WebLog.Threads);
            Assert.Equal(31, root.Logs.WebLog.BatchBytes);
            Assert.Equal(root.Logs.WebLog.SendPeriod, TimeSpan.FromSeconds(32));
            Assert.Equal(33, root.Logs.WebLog.QueueBytes);

            // internalLog
            Assert.False(root.Logs.InternalLog.Disable);
            Assert.Equal(Encoding.UTF8, root.Logs.InternalLog.Encoding);
            Assert.Equal(@"#appDir\Logs\#date\internal_#date_#hour.txt", root.Logs.InternalLog.FilePath);
            Assert.Equal(LogLevel.Info, root.Logs.InternalLog.MinLevel);
            Assert.False(root.Logs.InternalLog.DeleteOldFileOnStartup);

            // auto create events
            Assert.True(root.Logs.AutoCreateEvents.Disable);

            // eventManager
            Assert.True(root.Events.EventManager.Disabled);
            Assert.Equal(root.Events.EventManager.SendPeriod, TimeSpan.FromSeconds(23));
            Assert.Equal(50000001, root.Events.EventManager.QueueBytes);
            Assert.Equal(7, root.Events.EventManager.Threads);
            Assert.Equal(1002, root.Events.EventManager.MaxJoin);
            Assert.Equal(1001, root.Events.EventManager.MaxSend);

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
            Assert.Equal("key-yyy", root.Access.SecretKey);
            Assert.Equal(root.Access.WaitOnError, TimeSpan.FromSeconds(60));
            Assert.Null(root.Access.Url);

            // defaultComponent
            Assert.NotNull(root.DefaultComponent);
            Assert.Equal(new Guid("3b1377df-0caf-48c8-baa9-7a61be6b8f22"), root.DefaultComponent.Id);

            // webLog
            Assert.False(root.Logs.WebLog.Disable);
            Assert.Equal(3, root.Logs.WebLog.Threads);
            Assert.Equal(root.Logs.WebLog.BatchBytes, 100 * 1024); // 100 Kb
            Assert.Equal(root.Logs.WebLog.SendPeriod, TimeSpan.FromSeconds(5));
            Assert.Equal(root.Logs.WebLog.QueueBytes, 1024 * 1024 * 100); // 100 Мбайт

            // internalLog
            Assert.False(root.Logs.InternalLog.Disable);
            Assert.Equal(root.Logs.InternalLog.Encoding, Encoding.UTF8);
            Assert.Equal(@"#appDir\Logs\Zidium_Internal_#date.txt", root.Logs.InternalLog.FilePath);
            Assert.Equal(LogLevel.Warning, root.Logs.InternalLog.MinLevel);
            Assert.False(root.Logs.InternalLog.DeleteOldFileOnStartup);

            // auto create events
            Assert.True(root.Logs.AutoCreateEvents.Disable);

            // event manager
            Assert.False(root.Events.EventManager.Disabled);
            Assert.Equal(1000, root.Events.EventManager.MaxJoin);
            Assert.Equal(1000, root.Events.EventManager.MaxSend);
            Assert.Equal(root.Events.EventManager.QueueBytes, 1024 * 1024 * 100); // 100Mb
            Assert.Equal(TimeSpan.FromSeconds(5), root.Events.EventManager.SendPeriod);
            Assert.Equal(1, root.Events.EventManager.Threads);

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
            Assert.Equal("key-yyy", root.Access.SecretKey);
            Assert.Equal(root.Access.WaitOnError, TimeSpan.FromSeconds(60));
            Assert.Null(root.Access.Url);

            // defaultComponent
            Assert.NotNull(root.DefaultComponent);
            Assert.Equal(new Guid("3b1377df-0caf-48c8-baa9-7a61be6b8f22"), root.DefaultComponent.Id);

            // webLog
            Assert.False(root.Logs.WebLog.Disable);
            Assert.Equal(3, root.Logs.WebLog.Threads);
            Assert.Equal(root.Logs.WebLog.BatchBytes, 100 * 1024); // 100 Kb
            Assert.Equal(root.Logs.WebLog.SendPeriod, TimeSpan.FromSeconds(5));
            Assert.Equal(root.Logs.WebLog.QueueBytes, 1024 * 1024 * 100); // 100 Мбайт

            // internalLog
            Assert.True(root.Logs.InternalLog.Disable);
            Assert.Equal(root.Logs.InternalLog.Encoding, Encoding.UTF8);
            Assert.Equal(@"#appDir\Logs\#appName_ZidiumInternal_#date.txt", root.Logs.InternalLog.FilePath);
            Assert.Equal(LogLevel.Warning, root.Logs.InternalLog.MinLevel);
            Assert.False(root.Logs.InternalLog.DeleteOldFileOnStartup);

            // auto create events
            Assert.True(root.Logs.AutoCreateEvents.Disable);

            // event manager
            Assert.False(root.Events.EventManager.Disabled);
            Assert.Equal(1000, root.Events.EventManager.MaxJoin);
            Assert.Equal(1000, root.Events.EventManager.MaxSend);
            Assert.Equal(root.Events.EventManager.QueueBytes, 1024 * 1024 * 100); // 100Mb
            Assert.Equal(TimeSpan.FromSeconds(5), root.Events.EventManager.SendPeriod);
            Assert.Equal(1, root.Events.EventManager.Threads);

            // componentEvent
            Assert.Equal(root.Events.DefaultValues.ComponentEvent.JoinInterval, TimeSpan.FromMinutes(5));

            // applicationError
            Assert.Equal(root.Events.DefaultValues.ApplicationError.JoinInterval, TimeSpan.FromMinutes(5));
        }
    }
}
