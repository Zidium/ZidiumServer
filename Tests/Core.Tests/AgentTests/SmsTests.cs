using System;
using System.Threading;
using NLog;
using Xunit;
using Zidium.Agent.AgentTasks.SendSms;
using Zidium.Storage;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Core.Tests.AgentTests
{
    public class SmsTests
    {
        [Fact]
        public void ProcessQueueTest()
        {
            var account = TestHelper.GetTestAccount();
            Guid smsId;

            using (var accountDbContext = account.GetAccountDbContext())
            {
                var sms = accountDbContext.SendSmsCommands.Add(new DbSendSmsCommand()
                {
                    Id = Guid.NewGuid(),
                    CreateDate = DateTime.Now,
                    Status = SmsStatus.InQueue,
                    Body = "TEST SMS " + Guid.NewGuid(),
                    Phone = "+7 916 123-45-67"
                });
                accountDbContext.SaveChanges();
                smsId = sms.Id;
            }

            // обработаем очередь
            var apiId = "00000000-1111-2222-3333-444444444444";
            var from = "";
            var processor = new SendSmsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), apiId, from);
            processor.FakeMode = true;
            processor.Process(account.Id, smsId);

            // проверим, что sms отправлено
            using (var accountDbContext = account.GetAccountDbContext())
            {
                var sms = accountDbContext.SendSmsCommands.Find(smsId);
                Assert.Null(sms.ErrorMessage);
                Assert.Equal(SmsStatus.Sent, sms.Status);
            }
        }

    }
}
