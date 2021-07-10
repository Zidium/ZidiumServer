using System;

namespace Zidium.Storage
{
    public interface IUserContactRepository
    {
        void Add(UserContactForAdd entity);

        void Add(UserContactForAdd[] entities);

        void Update(UserContactForUpdate entity);

        void Delete(Guid id);

        UserContactForRead GetOneById(Guid id);

        UserContactForRead[] GetByUserId(Guid userId);

        UserContactForRead[] GetByType(Guid userId, UserContactType type);

    }
}
