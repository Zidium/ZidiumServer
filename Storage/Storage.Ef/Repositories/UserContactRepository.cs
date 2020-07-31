using System;
using System.Linq;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class UserContactRepository : IUserContactRepository
    {
        public UserContactRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(UserContactForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.UserContacts.Add(new DbUserContact()
                {
                    Id = entity.Id,
                    Value = entity.Value,
                    CreateDate = entity.CreateDate,
                    UserId = entity.UserId,
                    Type = entity.Type
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Add(UserContactForAdd[] entities)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                foreach (var entity in entities)
                {
                    contextWrapper.Context.UserContacts.Add(new DbUserContact()
                    {
                        Id = entity.Id,
                        Value = entity.Value,
                        CreateDate = entity.CreateDate,
                        UserId = entity.UserId,
                        Type = entity.Type
                    });
                }

                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(UserContactForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var contact = DbGetOneById(entity.Id);

                if (entity.Type.Changed())
                    contact.Type = entity.Type.Get();

                if (entity.Value.Changed())
                    contact.Value = entity.Value.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public void Delete(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var contact = DbGetOneById(id);
                contextWrapper.Context.UserContacts.Remove(contact);
                contextWrapper.Context.SaveChanges();
            }
        }

        public UserContactForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public UserContactForRead[] GetByUserId(Guid userId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UserContacts.AsNoTracking()
                    .Where(t => t.UserId == userId)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public UserContactForRead[] GetByType(Guid userId, UserContactType type)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UserContacts.AsNoTracking()
                    .Where(t => t.UserId == userId && t.Type == type)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        private DbUserContact DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UserContacts.Find(id);
            }
        }

        private DbUserContact DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Контакт {id} не найден");

            return result;
        }

        private UserContactForRead DbToEntity(DbUserContact entity)
        {
            if (entity == null)
                return null;

            return new UserContactForRead(entity.Id, entity.UserId, entity.Type, entity.Value, entity.CreateDate);
        }
    }
}
