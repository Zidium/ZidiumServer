using System;

namespace Zidium.Storage
{
    public interface IComponentRepository
    {
        void Add(ComponentForAdd entity);

        void Update(ComponentForUpdate entity);

        void Update(ComponentForUpdate[] entities);

        ComponentForRead GetOneById(Guid id);

        ComponentForRead GetOneOrNullById(Guid id);

        ComponentForRead[] GetMany(Guid[] ids);

        ComponentForRead GetRoot();

        ComponentForRead[] GetChilds(Guid id);

        ComponentForRead GetChild(Guid parentId, string systemName);

        Guid[] GetNotActualEventsStatusIds(DateTime now, int maxCount);

        ComponentGetAllIdsWithParentsInfo[] GetAllIdsWithParents();

        ComponentForRead[] GetByComponentTypeId(Guid componentTypeId);

        int GetCount();

        Guid[] GetAllIds();

        ComponentGetForNotificationsInfo[] GetForNotifications(Guid? componentId);

    }
}
