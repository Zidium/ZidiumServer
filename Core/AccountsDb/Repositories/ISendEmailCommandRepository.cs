using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public interface ISendEmailCommandRepository
    {
        void Add(SendEmailCommand command);

        SendEmailCommand Add(string to, string subject, string body, Guid? referenceId = null);

        List<SendEmailCommand> GetForSend(int maxCount);

        void MarkAsSendSuccessed(Guid id);

        void MarkAsSendFail(Guid id, string error);

        SendEmailCommand GetById(Guid id);

        IQueryable<SendEmailCommand> QueryAll();

    }
}
