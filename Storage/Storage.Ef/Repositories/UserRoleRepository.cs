using System;
using System.Linq;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class UserRoleRepository : IUserRoleRepository
    {
        public UserRoleRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(UserRoleForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.UserRoles.Add(new DbUserRole()
                {
                    Id = entity.Id,
                    UserId = entity.UserId,
                    RoleId = entity.RoleId
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Add(UserRoleForAdd[] entities)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                foreach (var entity in entities)
                {
                    contextWrapper.Context.UserRoles.Add(new DbUserRole()
                    {
                        Id = entity.Id,
                        UserId = entity.UserId,
                        RoleId = entity.RoleId
                    });
                }

                contextWrapper.Context.SaveChanges();
            }
        }

        public UserRoleForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public UserRoleForRead[] GetByUserId(Guid userId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UserRoles.AsNoTracking()
                    .Where(t => t.UserId == userId)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public void Delete(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var userRole = DbGetOneById(id);
                contextWrapper.Context.UserRoles.Remove(userRole);
                contextWrapper.Context.SaveChanges();
            }
        }

        private DbUserRole DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UserRoles.Find(id);
            }
        }

        private DbUserRole DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Привязка роли {id} не найдена");

            return result;
        }

        private UserRoleForRead DbToEntity(DbUserRole entity)
        {
            if (entity == null)
                return null;

            return new UserRoleForRead(entity.Id, entity.UserId, entity.RoleId);
        }
    }
}
