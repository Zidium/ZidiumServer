using System;

namespace Zidium.Storage
{
    public interface IUserRepository
    {
        void Add(UserForAdd entity);

        void Update(UserForUpdate entity);

        UserForRead GetOneById(Guid id);

        UserForRead GetOneOrNullById(Guid id);

        UserForRead[] GetAll();

        UserForRead[] GetMany(Guid[] ids);

        UserGetForNotificationsInfo[] GetForNotifications();

        UserForRead GetOneOrNullByLogin(string login);

    }
}
