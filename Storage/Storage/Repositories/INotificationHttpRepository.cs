using System;

namespace Zidium.Storage
{
    public interface INotificationHttpRepository
    {
        void Add(NotificationHttpForAdd entity);

        void Update(NotificationHttpForUpdate entity);

        NotificationHttpForRead GetByNotificationId(Guid notificationId);

    }
}
