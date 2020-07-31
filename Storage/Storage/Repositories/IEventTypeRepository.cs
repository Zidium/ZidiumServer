using System;

namespace Zidium.Storage
{
    public interface IEventTypeRepository
    {
        void Add(EventTypeForAdd entity);

        void Update(EventTypeForUpdate entity);

        EventTypeForRead GetOneById(Guid id);

        EventTypeForRead GetOneOrNullById(Guid id);

        EventTypeForRead GetOneOrNullByCategoryAndName(EventCategory category, string systemName);

        EventTypeForRead GetOneOrNullBySystemName(string systemName);

        EventTypeForRead[] GetMany(Guid[] ids);

        EventTypeForRead[] Filter(
            EventImportance? importance,
            EventCategory? category,
            string search, 
            int maxCount);

    }
}
