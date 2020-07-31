using System;

namespace Zidium.Storage
{
    public interface IArchivedStatusRepository
    {
        void Add(ArchivedStatusForAdd entity);

        void Delete(long id);

        ArchivedStatusGetForNotificationsInfo[] GetForNotifications(Guid? componentId, int maxCount);
    }
}
