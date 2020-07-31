using System;

namespace Zidium.Storage
{
    public interface INotificationRepository
    {
        void Add(NotificationForAdd entity);

        void Update(NotificationForUpdate entity);

        void DeleteBySubscriptionId(Guid subscriptionId);

        NotificationForRead GetOneById(Guid id);

        NotificationForRead GetOneOrNullById(Guid id);

        NotificationForRead[] GetForSend(SubscriptionChannel[] channels, Guid? componentId, int maxCount);

        NotificationForRead[] Filter(
            Guid? componentId,
            DateTime? fromDate,
            DateTime? toDate,
            EventCategory? category,
            SubscriptionChannel? channel,
            NotificationStatus? status,
            Guid? userId, 
            int maxCount);

    }
}
