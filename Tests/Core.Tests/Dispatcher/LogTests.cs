using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Dispatcher
{
    public class LogTests : BaseTest
    {
        [Fact]
        public void GetLogsByComponentIdOnlyTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = TestHelper.CreateRandomComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Message"
            }).Check();

            var logs = dispatcher.GetLogs(account.Id, new Api.GetLogsRequestData()
            {
                ComponentId = component.Info.Id
            }).Data;

            Assert.Equal(1, logs.Length);
            var log = logs[0];
            Assert.Equal("Message", log.Message);
            
        }

        [Fact]
        public void GetLogsByDateFromTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = TestHelper.CreateRandomComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            var date = DateTimeHelper.TrimMs(DateTime.Now);

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Message 1",
                Date = date
            }).Check();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Message 2",
                Date = date.AddSeconds(-1)
            }).Check();

            var logs = dispatcher.GetLogs(account.Id, new Api.GetLogsRequestData()
            {
                ComponentId = component.Info.Id,
                From = date
            }).Data;

            Assert.Equal(1, logs.Length);
            var log = logs[0];
            Assert.Equal("Message 1", log.Message);

        }

        [Fact]
        public void GetLogsByDateToTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = TestHelper.CreateRandomComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            var date = DateTimeHelper.TrimMs(DateTime.Now);

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Message 1",
                Date = date.AddSeconds(-1)
            }).Check();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Message 2",
                Date = date
            }).Check();

            var logs = dispatcher.GetLogs(account.Id, new Api.GetLogsRequestData()
            {
                ComponentId = component.Info.Id,
                To = date
            }).Data;

            Assert.Equal(1, logs.Length);
            var log = logs[0];
            Assert.Equal("Message 1", log.Message);

        }

        [Fact]
        public void GetLogsByLevelsTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = TestHelper.CreateRandomComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Message 1"
            }).Check();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Error,
                Message = "Message 2"
            }).Check();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Warning,
                Message = "Message 3"
            }).Check();

            var logs = dispatcher.GetLogs(account.Id, new Api.GetLogsRequestData()
            {
                ComponentId = component.Info.Id,
                Levels = new LogLevel[] { LogLevel.Info, LogLevel.Error }
            }).Data;

            Assert.Equal(2, logs.Length);
            var log1 = logs.FirstOrDefault(t => t.Level == LogLevel.Info);
            Assert.NotNull(log1);
            Assert.Equal("Message 1", log1.Message);
            var log2 = logs.FirstOrDefault(t => t.Level == LogLevel.Error);
            Assert.NotNull(log2);
            Assert.Equal("Message 2", log2.Message);
        }

        [Fact]
        public void GetLogsByContextTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = TestHelper.CreateRandomComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Message 1",
                Context = "Context.Suffix"
            }).Check();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Message 2",
                Context = "Prefix.Context"
            }).Check();

            var logs = dispatcher.GetLogs(account.Id, new Api.GetLogsRequestData()
            {
                ComponentId = component.Info.Id,
                Context = "context"
            }).Data;

            Assert.Equal(1, logs.Length);
            var log = logs[0];
            Assert.Equal("Message 1", log.Message);
        }

        [Fact]
        public void GetLogsByMessageTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = TestHelper.CreateRandomComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Prefix.Message.Suffix"
            }).Check();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Prefix.Another.Suffix"
            }).Check();

            var logs = dispatcher.GetLogs(account.Id, new Api.GetLogsRequestData()
            {
                ComponentId = component.Info.Id,
                Message = "message"
            }).Data;

            Assert.Equal(1, logs.Length);
            var log = logs[0];
            Assert.Equal("Prefix.Message.Suffix", log.Message);
        }

        [Fact]
        public void GetLogsByPropertyNameTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = TestHelper.CreateRandomComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Message 1",
                Properties = new List<Api.ExtentionPropertyDto>()
                {
                    new Api.ExtentionPropertyDto()
                    {
                        Name = "Name 1",
                        Value = "Value 1",
                        Type = DataType.String
                    }
                }
            }).Check();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Message 2",
                Properties = new List<Api.ExtentionPropertyDto>()
                {
                    new Api.ExtentionPropertyDto()
                    {
                        Name = "Name 2",
                        Value = "Value 2",
                        Type = DataType.String
                    }
                }
            }).Check();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Message 3"
            });

            var logs = dispatcher.GetLogs(account.Id, new Api.GetLogsRequestData()
            {
                ComponentId = component.Info.Id,
                PropertyName = "name 1"
            }).Data;

            Assert.Equal(1, logs.Length);
            var log = logs[0];
            Assert.Equal("Message 1", log.Message);
        }

        [Fact]
        public void GetLogsByPropertyValueTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = TestHelper.CreateRandomComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Message 1",
                Properties = new List<Api.ExtentionPropertyDto>()
                {
                    new Api.ExtentionPropertyDto()
                    {
                        Name = "Name 1",
                        Value = "Value 1",
                        Type = DataType.String
                    }
                }
            }).Check();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Message 2",
                Properties = new List<Api.ExtentionPropertyDto>()
                {
                    new Api.ExtentionPropertyDto()
                    {
                        Name = "Name 2",
                        Value = "Value 2",
                        Type = DataType.String
                    }
                }
            }).Check();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Message 3"
            }).Check();

            var logs = dispatcher.GetLogs(account.Id, new Api.GetLogsRequestData()
            {
                ComponentId = component.Info.Id,
                PropertyValue = "value 1"
            }).Data;

            Assert.Equal(1, logs.Length);
            var log = logs[0];
            Assert.Equal("Message 1", log.Message);
        }

        [Fact]
        public void GetLogsByPropertyNameAndValueTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = TestHelper.CreateRandomComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Message 1",
                Properties = new List<Api.ExtentionPropertyDto>()
                {
                    new Api.ExtentionPropertyDto()
                    {
                        Name = "Name 1",
                        Value = "Value 1",
                        Type = DataType.String
                    }
                }
            }).Check();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Message 2",
                Properties = new List<Api.ExtentionPropertyDto>()
                {
                    new Api.ExtentionPropertyDto()
                    {
                        Name = "Name 1",
                        Value = "Value 2",
                        Type = DataType.String
                    }
                }
            }).Check();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Message 3",
                Properties = new List<Api.ExtentionPropertyDto>()
                {
                    new Api.ExtentionPropertyDto()
                    {
                        Name = "Name 2",
                        Value = "Value 1",
                        Type = DataType.String
                    }
                }
            }).Check();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Message 4"
            }).Check();

            var logs = dispatcher.GetLogs(account.Id, new Api.GetLogsRequestData()
            {
                ComponentId = component.Info.Id,
                PropertyName = "name 1",
                PropertyValue = "value 1"
            }).Data;

            Assert.Equal(1, logs.Length);
            var log = logs[0];
            Assert.Equal("Message 1", log.Message);
        }

        [Fact]
        public void SendLogWithLongPropertyNameTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = TestHelper.CreateRandomComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "Message",
                Properties = new List<Api.ExtentionPropertyDto>()
                {
                    new Api.ExtentionPropertyDto()
                    {
                        Name = RandomHelper.GetRandomStringAZ09(1000, 1000),
                        Value = "Value",
                        Type = DataType.String
                    }
                }
            }).Check();

            var logs = dispatcher.GetLogs(account.Id, new Api.GetLogsRequestData()
            {
                ComponentId = component.Info.Id
            }).Data;

            Assert.Equal(1, logs.Length);
            var property = logs[0].Properties[0];
            Assert.Equal(100, property.Name.Length);
        }

        [Fact]
        public void SendLogWithZeroesSymbols()
        {
            var account = TestHelper.GetTestAccount();
            var component = TestHelper.CreateRandomComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            dispatcher.SendLog(account.Id, new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = LogLevel.Info,
                Message = "AAA" + "\x00" + "BBB",
                Properties = new List<Api.ExtentionPropertyDto>()
                {
                    new Api.ExtentionPropertyDto()
                    {
                        Name = "Property_" + "\x00" + "_Test",
                        Value = "AAA" + "\x00" + "BBB",
                        Type = DataType.String
                    }
                }
            }).Check();
        }

    }
}
