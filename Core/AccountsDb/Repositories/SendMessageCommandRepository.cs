using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    public class SendMessageCommandRepository : ISendMessageCommandRepository
    {
        protected AccountDbContext Context { get; set; }

        public SendMessageCommandRepository(AccountDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public void Add(SendMessageCommand command)
        {
            if (command.Id == Guid.Empty)
                command.Id = Guid.NewGuid();
            command.Status = MessageStatus.InQueue;
            command.CreateDate = DateTime.Now;
            Context.SendMessageCommands.Add(command);
            Context.SaveChanges();
        }

        public List<SendMessageCommand> GetForSend(SubscriptionChannel channel, int maxCount)
        {
            return Context.SendMessageCommands
                .Where(x => x.Channel == channel && x.Status == MessageStatus.InQueue)
                .OrderBy(x => x.CreateDate)
                .Take(maxCount)
                .ToList();
        }

        public void MarkAsSendSuccessed(Guid id)
        {
            var smsCommand = GetById(id);
            if (smsCommand != null)
            {
                smsCommand.Status = MessageStatus.Sent;
                smsCommand.SendDate = DateTime.Now;
                smsCommand.ErrorMessage = null;
                Context.SaveChanges();
            }
        }

        public void MarkAsSendFail(Guid id, string error)
        {
            var smsCommand = GetById(id);
            if (smsCommand != null)
            {
                smsCommand.Status = MessageStatus.Error;
                smsCommand.SendDate = DateTime.Now;
                smsCommand.ErrorMessage = error;
                Context.SaveChanges();
            }
        }

        public SendMessageCommand GetById(Guid id)
        {
            return Context.SendMessageCommands.Find(id);
        }

        public IQueryable<SendMessageCommand> QueryAll()
        {
            return Context.SendMessageCommands;
        }
    }
}