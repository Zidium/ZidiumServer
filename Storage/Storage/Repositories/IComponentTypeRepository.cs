using System;

namespace Zidium.Storage
{
    public interface IComponentTypeRepository
    {
        void Add(ComponentTypeForAdd entity);

        void Update(ComponentTypeForUpdate entity);

        ComponentTypeForRead GetOneById(Guid id);

        ComponentTypeForRead GetOneOrNullById(Guid id);

        ComponentTypeForRead GetOneOrNullBySystemName(string systemName);

        ComponentTypeForRead[] GetMany(Guid[] ids);

        int GetNonSystemCount();

        ComponentTypeForRead[] Filter(string search, int maxCount);

    }
}
