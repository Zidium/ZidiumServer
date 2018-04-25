using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public class SendSmsCommandRepository : ISendSmsCommandRepository
    {
        protected AccountDbContext Context { get; set; }

        public SendSmsCommandRepository(AccountDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public void Add(SendSmsCommand command)
        {
            if (command.Id == Guid.Empty)
                command.Id = Guid.NewGuid();
            command.Status = SmsStatus.InQueue;
            command.CreateDate = DateTime.Now;
            Context.SendSmsCommands.Add(command);
            Context.SaveChanges();
        }

        public SendSmsCommand Add(string phone, string body, Guid? referenceId = null)
        {
            var result = new SendSmsCommand()
            {
                Body = body,
                Phone = phone,
                ReferenceId = referenceId
            };
            Add(result);
            return result;
        }

        public List<SendSmsCommand> GetForSend(int maxCount)
        {
            return Context.SendSmsCommands
                .Where(x => x.Status == SmsStatus.InQueue)
                .OrderBy(x => x.CreateDate)
                .Take(maxCount)
                .ToList();
        }

        public void MarkAsSendSuccessed(Guid id, string externalId)
        {
            var smsCommand = GetById(id);
            if (smsCommand != null)
            {
                smsCommand.Status = SmsStatus.Sent;
                smsCommand.SendDate = DateTime.Now;
                smsCommand.ErrorMessage = null;
                smsCommand.ExternalId = externalId;
                Context.SaveChanges();
            }
        }

        public void MarkAsSendFail(Guid id, string error)
        {
            var smsCommand = GetById(id);
            if (smsCommand != null)
            {
                smsCommand.Status = SmsStatus.Error;
                smsCommand.SendDate = DateTime.Now;
                smsCommand.ErrorMessage = error;
                Context.SaveChanges();
            }
        }

        public SendSmsCommand GetById(Guid id)
        {
            return Context.SendSmsCommands.Find(id);
        }

        public IQueryable<SendSmsCommand> QueryAll()
        {
            return Context.SendSmsCommands;
        }
    }
}
