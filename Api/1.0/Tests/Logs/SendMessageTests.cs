using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Api;
using Zidium.Core.Common;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.Logs
{
    
    public class SendMessageTests
    {
        [Fact]
        public void SendLogTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var root = client.GetRootComponentControl();

            var levels = new[]
            {
                LogLevel.Debug, 
                LogLevel.Trace, 
                LogLevel.Info, 
                LogLevel.Warning, 
                LogLevel.Error, 
                LogLevel.Fatal
            };

            var date = RandomHelper.GetRandomDate(
                new DateTime(1980, 1, 1), 
                new DateTime(2014, 1, 1));

            date = TestHelper.GetRoundDateTime(date);

            // отправляем сообщения каждого уровня
            var logDate = date;
            var messages = new List<SendLogData>();
            foreach (var level in levels)
            {
                var message = new SendLogData()
                {
                    ComponentId = root.Info.Id,
                    Date = logDate,
                    Message = "test message " + level,
                    Level = level,
                    Context = Guid.NewGuid().ToString()
                };
                messages.Add(message);
                TestHelper.InitRandomProperties(message.Properties);
                var response = client.ApiService.SendLog(message);
                Assert.True(response.Success);
                logDate = logDate.AddSeconds(1);
            }

            // проверим, что все сообщения записались
            var findMessage = new GetLogsFilter()
            {
                From = date,
                To = date.AddSeconds(levels.Length),
                Levels = levels.ToList(),
                MaxCount = levels.Length
            };
            var findLogsResponse = root.GetLogs(findMessage);
            Assert.True(findLogsResponse.Success);
            Assert.Equal(levels.Length, findLogsResponse.Data.Count);
            logDate = date;
            foreach (var level in levels)
            {
                var logRow = findLogsResponse.Data.FirstOrDefault(x => x.Level == level);
                var message = messages.First(x => x.Level == level);
                Assert.NotNull(logRow);
                Assert.Equal("test message " + level, logRow.Message);
                Assert.Equal(logDate, logRow.Date);
                Assert.Equal(message.Context, logRow.Context);
                Assert.True(logRow.Properties.Count > 0);
                TestHelper.CheckExtentionProperties(logRow.Properties, message.Properties);
                logDate = logDate.AddSeconds(1);
            }
        }
    }
}
