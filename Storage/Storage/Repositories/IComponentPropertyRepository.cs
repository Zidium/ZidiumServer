using System;

namespace Zidium.Storage
{
    public interface IComponentPropertyRepository
    {
        void Add(ComponentPropertyForAdd entity);

        void Update(ComponentPropertyForUpdate entity);

        ComponentPropertyForRead[] GetByComponentId(Guid componentId);

        ComponentPropertyForRead GetOneById(Guid id);

        void Delete(Guid id);

    }
}
