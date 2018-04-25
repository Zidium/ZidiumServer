using System;
using System.Threading;
using NLog;
using Zidium.Agent.AgentTasks.SendEMails;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.TestTools;

namespace Zidium.Core.Tests.AgentTests
{
    public class EmailsTests
    {
        [Fact]
        public void ProcessQueueTest()
        {
            var account = TestHelper.GetTestAccount();
            Guid emailId;

            using (var accountDbContext = account.CreateAccountDbContext())
            {

                // создадим письмо
                var email = new SendEmailCommand()
                {
                    Body = "test body",
                    CreateDate = DateTime.Now,
                    From = "zidium@yandex.ru",
                    Id = Guid.NewGuid(),
                    Status = EmailStatus.InQueue,
                    Subject = "app test subject",
                    IsHtml = false,
                    To = "zem_ao@mail.ru"
                };
                accountDbContext.SendEmailCommands.Add(email);
                accountDbContext.SaveChanges();
                emailId = email.Id;
            }

            // обработаем очередь
            string server = "mail.server.ru";
            int port = 465;
            bool useMailKit = true;
            bool useSsl = true;
            string login = "robot";
            string from = "robor@mail.server.ru";
            string password = "12345";
            var processor = new SendEmailsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), server, port, login, from, password, useMailKit, useSsl);
            processor.FakeMode = true;
            processor.Process(account.Id, emailId);

            // проверим, что письмо отправлено
            using (var accountDbContext = account.CreateAccountDbContext())
            {
                var email = accountDbContext.GetSendEmailCommandRepository().GetById(emailId);
                Assert.Null(email.ErrorMessage);
                Assert.Equal(EmailStatus.Sent, email.Status);
            }
        }
    }
}
