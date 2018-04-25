using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public interface ISendSmsCommandRepository
    {
        void Add(SendSmsCommand command);

        SendSmsCommand Add(string phone, string body, Guid? referenceId = null);

        List<SendSmsCommand> GetForSend(int maxCount);

        void MarkAsSendSuccessed(Guid id, string externalId);

        void MarkAsSendFail(Guid id, string error);

        SendSmsCommand GetById(Guid id);

        IQueryable<SendSmsCommand> QueryAll();

    }
}
