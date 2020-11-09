using System;
using System.Threading;
using Zidium.Agent.AgentTasks.DeleteLogs;
using Xunit;
using Zidium.Core.Api;
using Zidium.Storage;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Core.Single.Tests
{
    public class DeleteLogsTests : BaseTest
    {
        [Fact]
        public void DeleteLogsTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            // Установим время хранения лога 30 дней
            var dispatcher = DispatcherHelper.GetDispatcherService();

            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data.Hard;

            limits.LogMaxDays = 30;

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits,
                    Type = TariffLimitType.Hard
                }
            }).Check();

            // Выполним предварительную очистку лога
            var processor = new DeleteLogsProcessor(NLog.LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.Process(account.Id);
            Assert.Null(processor.DbProcessor.FirstException);

            // Добавим одну старую и одну новую запись лога

            var oldDate = DateTime.Now.AddDays(-31);

            using (var accountDbContext = account.GetAccountDbContext())
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
            processor.Process(account.Id);

            // Проверим, что нет ошибок и удалена 1 запись

            Assert.Null(processor.DbProcessor.FirstException);
            Assert.Equal(1, processor.DeletedLogsCount);
            Assert.Equal(1, processor.DeletedPropertiesCount);
        }
    }
}
