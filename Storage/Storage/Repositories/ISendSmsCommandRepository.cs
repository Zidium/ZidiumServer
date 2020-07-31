using System;

namespace Zidium.Storage
{
    public interface ISendSmsCommandRepository
    {
        void Add(SendSmsCommandForAdd entity);

        SendSmsCommandForRead[] GetForSend(int maxCount);

        void MarkAsSendSuccessed(Guid id, DateTime now, string externalId);

        void MarkAsSendFail(Guid id, DateTime now, string error);

    }
}
