using System;

namespace Zidium.Storage
{
    public interface IBulbRepository
    {
        void Add(BulbForAdd entity);

        void Update(BulbForUpdate entity);

        void Update(BulbForUpdate[] entities);

        BulbForRead GetOneById(Guid id);

        BulbForRead GetOneOrNullById(Guid id);

        BulbForRead[] GetMany(Guid[] ids);

        BulbGetForNotificationsInfo[] GetForNotifications();

    }
}
