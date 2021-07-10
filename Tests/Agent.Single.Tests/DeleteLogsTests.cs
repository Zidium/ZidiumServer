using System;
using System.Threading;
using Zidium.Agent.AgentTasks.DeleteLogs;
using Xunit;
using Zidium.Storage.Ef;
using Zidium.TestTools;
using Zidium.Api.Dto;
using Microsoft.Extensions.Logging.Abstractions;

namespace Zidium.Agent.Single.Tests
{
    public class DeleteLogsTests : BaseTest
    {
        [Fact]
        public void DeleteLogsTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            // Выполним предварительную очистку лога
            var processor = new DeleteLogsProcessor(NullLogger.Instance, new CancellationToken());
            processor.Process();

            // Добавим одну старую и одну новую запись лога

            var oldDate = DateTime.Now.AddDays(-31);

            using (var accountDbContext = account.GetDbContext())
            {
                var message = "test log message " + Guid.NewGuid();

                var log = new DbLog()
                {
                    ComponentId = component.Info.Id,
                    Id = Guid.NewGuid(),
                    Date = oldDate,
                    Level = LogLevel.Debug,
                    Message = message,
                    ParametersCount = 1
                };
                log.Parameters.Add(new DbLogProperty()
                {
                    Id = Guid.NewGuid(),
                    LogId = log.Id,
                    Log = log,
                    DataType = DataType.String,
                    Name = "test name",
                    Value = "test value"
                });
                accountDbContext.Logs.Add(log);

                log = new DbLog()
                {
                    ComponentId = component.Info.Id,
                    Id = Guid.NewGuid(),
                    Date = DateTime.Now,
                    Level = LogLevel.Debug,
                    Message = message,
                    ParametersCount = 0
                };
                accountDbContext.Logs.Add(log);

                accountDbContext.SaveChanges();
            }

            account.SaveAllCaches();

            // Удалим старые записи лога
            processor.Process();

            // Проверим, что удалена 1 запись
            Assert.Equal(1, processor.DeletedLogsCount);
            Assert.Equal(1, processor.DeletedPropertiesCount);
        }
    }
}
