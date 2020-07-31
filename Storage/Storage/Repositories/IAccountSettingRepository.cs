using System;

namespace Zidium.Storage
{
    public interface IAccountSettingRepository
    {
        void Add(AccountSettingForAdd entity);

        void Update(AccountSettingForUpdate entity);

        AccountSettingForRead GetOneById(Guid id);

        AccountSettingForRead GetOneOrNullById(Guid id);

        AccountSettingForRead GetOneOrNullByName(string name);

    }
}
