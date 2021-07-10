using System;

namespace Zidium.Storage
{
    public interface ISendEmailCommandRepository
    {
        void Add(SendEmailCommandForAdd entity);

        SendEmailCommandForRead GetOneById(Guid id);

        SendEmailCommandForRead[] GetForSend(int maxCount);

        void MarkAsSendSuccessed(Guid id, DateTime now);

        void MarkAsSendFail(Guid id, DateTime now, string error);
    }
}
