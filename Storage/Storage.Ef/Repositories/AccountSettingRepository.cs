using System;
using System.Linq;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class AccountSettingRepository : IAccountSettingRepository
    {
        public AccountSettingRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(AccountSettingForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.AccountSettings.Add(new DbAccountSetting()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Value = entity.Value
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(AccountSettingForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var accountSetting = DbGetOneById(entity.Id);

                if (entity.Value.Changed())
                    accountSetting.Value = entity.Value.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public AccountSettingForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public AccountSettingForRead GetOneOrNullById(Guid id)
        {
            return DbToEntity(DbGetOneOrNullById(id));
        }

        public AccountSettingForRead GetOneOrNullByName(string name)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.AccountSettings.FirstOrDefault(t => t.Name.ToLower() == name.ToLower()));
            }
        }

        private DbAccountSetting DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.AccountSettings.Find(id);
            }
        }

        private DbAccountSetting DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Настройка {id} не найдена");

            return result;
        }

        private AccountSettingForRead DbToEntity(DbAccountSetting entity)
        {
            if (entity == null)
                return null;

            return new AccountSettingForRead(entity.Id, entity.Name, entity.Value);
        }
    }
}
