using System;

namespace Zidium.Storage
{
    public interface IRoleRepository
    {
        void Add(RoleForAdd entity);

        RoleForRead GetOneById(Guid id);

        RoleForRead GetOneOrNullById(Guid id);

        RoleForRead[] GetByUserId(Guid userId);

        RoleForRead[] GetAll();

    }
}
