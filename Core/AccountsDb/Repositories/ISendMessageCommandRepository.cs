using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    public interface ISendMessageCommandRepository
    {
        void Add(SendMessageCommand command);

        List<SendMessageCommand> GetForSend(SubscriptionChannel channel, int maxCount);

        void MarkAsSendSuccessed(Guid id);

        void MarkAsSendFail(Guid id, string error);

        SendMessageCommand GetById(Guid id);

        IQueryable<SendMessageCommand> QueryAll();

    }
}