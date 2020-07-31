using System;
using System.Data.Entity;
using System.Linq;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class RoleRepository : IRoleRepository
    {
        public RoleRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(RoleForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.Roles.Add(new DbRole()
                {
                    Id = entity.Id,
                    DisplayName = entity.DisplayName,
                    SystemName = entity.SystemName
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public RoleForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public RoleForRead GetOneOrNullById(Guid id)
        {
            return DbToEntity(DbGetOneOrNullById(id));
        }

        public RoleForRead[] GetByUserId(Guid userId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UserRoles.AsNoTracking().AsQueryable()
                    .Include(t => t.Role)
                    .Where(t => t.UserId == userId)
                    .Select(t => t.Role)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public RoleForRead[] GetAll()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Roles.AsNoTracking()
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        private DbRole DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Roles.Find(id);
            }
        }

        private DbRole DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Роль {id} не найдена");

            return result;
        }

        private RoleForRead DbToEntity(DbRole entity)
        {
            if (entity == null)
                return null;

            return new RoleForRead(entity.Id, entity.SystemName, entity.DisplayName);
        }
    }
}
