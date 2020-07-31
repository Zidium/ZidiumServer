using System;
using System.Linq;
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
                    FirstName = entity.FirstName,
                    InArchive = entity.InArchive,
                    LastName = entity.LastName,
                    Login = entity.Login,
                    MiddleName = entity.MiddleName,
                    PasswordHash = entity.PasswordHash,
                    Post = entity.Post,
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

                if (entity.PasswordHash.Changed())
                    user.PasswordHash = entity.PasswordHash.Get();

                if (entity.FirstName.Changed())
                    user.FirstName = entity.FirstName.Get();

                if (entity.LastName.Changed())
                    user.LastName = entity.LastName.Get();

                if (entity.MiddleName.Changed())
                    user.MiddleName = entity.MiddleName.Get();

                if (entity.DisplayName.Changed())
                    user.DisplayName = entity.DisplayName.Get();

                if (entity.Post.Changed())
                    user.Post = entity.Post.Get();

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

            return new UserForRead(entity.Id, entity.Login, entity.PasswordHash, entity.FirstName, entity.LastName,
                entity.MiddleName, entity.DisplayName, entity.CreateDate, entity.Post, entity.InArchive,
                entity.SecurityStamp);
        }
    }
}
