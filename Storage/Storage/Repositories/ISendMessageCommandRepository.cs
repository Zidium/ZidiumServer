using System;

namespace Zidium.Storage
{
    public interface ISendMessageCommandRepository
    {
        void Add(SendMessageCommandForAdd entity);

        SendMessageCommandForRead GetOneById(Guid id);

        SendMessageCommandForRead[] GetForSend(SubscriptionChannel channel, int maxCount);

        void MarkAsSendSuccessed(Guid id, DateTime now);

        void MarkAsSendFail(Guid id, DateTime now, string error);

    }
}
