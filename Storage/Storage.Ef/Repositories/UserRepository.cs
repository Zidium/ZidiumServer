using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class UserRepository : IUserRepository
    {
        public UserRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(UserForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.Users.Add(new DbUser()
                {
                    Id = entity.Id,
                    DisplayName = entity.DisplayName,
                    CreateDate = entity.CreateDate,
                    InArchive = entity.InArchive,
                    Login = entity.Login,
                    EMail = entity.EMail,
                    PasswordHash = entity.PasswordHash,
                    SecurityStamp = entity.SecurityStamp
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(UserForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var user = DbGetOneById(entity.Id);

                if (entity.Login.Changed())
                    user.Login = entity.Login.Get();

                if (entity.EMail.Changed())
                    user.EMail = entity.EMail.Get();

                if (entity.PasswordHash.Changed())
                    user.PasswordHash = entity.PasswordHash.Get();

                if (entity.DisplayName.Changed())
                    user.DisplayName = entity.DisplayName.Get();

                if (entity.InArchive.Changed())
                    user.InArchive = entity.InArchive.Get();

                if (entity.SecurityStamp.Changed())
                    user.SecurityStamp = entity.SecurityStamp.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public UserForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public UserForRead GetOneOrNullById(Guid id)
        {
            return DbToEntity(DbGetOneOrNullById(id));
        }

        public UserForRead[] GetAll()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Users.AsNoTracking()
                    .Where(t => !t.InArchive)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public UserForRead[] GetMany(Guid[] ids)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Users.AsNoTracking()
                    .Where(t => ids.Contains(t.Id))
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public UserGetForNotificationsInfo[] GetForNotifications()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Users.AsNoTracking()
                    .Where(t => !t.InArchive)
                    .Select(t => new UserGetForNotificationsInfo()
                    {
                        Id = t.Id,
                        CreateDate = t.CreateDate,
                        Login = t.Login
                    })
                    .ToArray();
            }
        }

        public UserForRead GetOneOrNullByLogin(string login)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.Users.AsNoTracking()
                    .FirstOrDefault(t => !t.InArchive && t.Login.ToLower() == login.ToLower()));
            }
        }

        private DbUser DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Users.Find(id);
            }
        }

        private DbUser DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Пользователь {id} не найден");

            return result;
        }

        private UserForRead DbToEntity(DbUser entity)
        {
            if (entity == null)
                return null;

            return new UserForRead(entity.Id, entity.Login, entity.EMail, entity.PasswordHash,
                entity.DisplayName, entity.CreateDate, entity.InArchive,
                entity.SecurityStamp);
        }
    }
}
