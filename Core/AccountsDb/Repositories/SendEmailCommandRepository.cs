using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public class SendEmailCommandRepository : ISendEmailCommandRepository
    {
        protected AccountDbContext Context { get; set; }

        public SendEmailCommandRepository(AccountDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public void Add(SendEmailCommand command)
        {
            if (command.Id == Guid.Empty)
                command.Id = Guid.NewGuid();
            command.Status = EmailStatus.InQueue;
            command.CreateDate = DateTime.Now;
            Context.SendEmailCommands.Add(command);
            Context.SaveChanges(); // todo репозиторий не должен сохранять изменения
        }

        public SendEmailCommand Add(string to, string subject, string body, Guid? referenceId = null)
        {
            var result = new SendEmailCommand()
            {
                Body = body,
                IsHtml = true,
                Subject = subject,
                To = to,
                ReferenceId = referenceId
            };
            Add(result);
            return result;
        }

        public List<SendEmailCommand> GetForSend(int maxCount)
        {
            return Context.SendEmailCommands
                .Where(x => x.Status == EmailStatus.InQueue)
                .OrderBy(x => x.CreateDate)
                .Take(maxCount)
                .ToList();
        }

        public void MarkAsSendSuccessed(Guid id)
        {
            var email = GetById(id);
            if (email != null)
            {
                email.Status = EmailStatus.Sent;
                email.SendDate = DateTime.Now;
                email.ErrorMessage = null;
                Context.SaveChanges();
            }
        }

        public void MarkAsSendFail(Guid id, string error)
        {
            var email = GetById(id);
            if (email != null)
            {
                email.Status = EmailStatus.Error;
                email.SendDate = DateTime.Now;
                email.ErrorMessage = error;
                Context.SaveChanges();
            }
        }

        public SendEmailCommand GetById(Guid id)
        {
            return Context.SendEmailCommands.Find(id);
        }

        public IQueryable<SendEmailCommand> QueryAll()
        {
            return Context.SendEmailCommands;
        }
    }
}
