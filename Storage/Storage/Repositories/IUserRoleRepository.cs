using System;

namespace Zidium.Storage
{
    public interface IUserRoleRepository
    {
        void Add(UserRoleForAdd entity);

        void Add(UserRoleForAdd[] entities);

        UserRoleForRead GetOneById(Guid id);

        UserRoleForRead[] GetByUserId(Guid userId);

        void Delete(Guid id);

    }
}
