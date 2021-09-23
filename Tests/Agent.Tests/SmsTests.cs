using System;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Zidium.Agent.AgentTasks.SendSms;
using Zidium.Common;
using Zidium.Storage;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Agent.Tests
{
    public class SmsTests : BaseTest
    {
        [Fact]
        public void ProcessQueueTest()
        {
            var account = TestHelper.GetTestAccount();
            Guid smsId;

            using (var accountDbContext = account.GetDbContext())
            {
                var sms = new DbSendSmsCommand()
                {
                    Id = Ulid.NewUlid(),
                    CreateDate = DateTime.Now,
                    Status = SmsStatus.InQueue,
                    Body = "TEST SMS " + Ulid.NewUlid(),
                    Phone = "+7 916 123-45-67"
                };

                accountDbContext.SendSmsCommands.Add(sms);
                accountDbContext.SaveChanges();
                smsId = sms.Id;
            }

            // обработаем очередь
            var apiId = "00000000-1111-2222-3333-444444444444";
            var from = "";
            var processor = new SendSmsProcessor(NullLogger.Instance, new CancellationToken(), apiId, from);
            processor.FakeMode = true;
            processor.Process(smsId);

            // проверим, что sms отправлено
            using (var accountDbContext = account.GetDbContext())
            {
                var sms = accountDbContext.SendSmsCommands.Find(smsId);
                Assert.Null(sms.ErrorMessage);
                Assert.Equal(SmsStatus.Sent, sms.Status);
            }
        }

    }
}
