using System;
using System.Linq;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class ComponentTypeRepository : IComponentTypeRepository
    {
        public ComponentTypeRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(ComponentTypeForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.ComponentTypes.Add(new DbComponentType()
                {
                    Id = entity.Id,
                    DisplayName = entity.DisplayName,
                    SystemName = entity.SystemName,
                    IsSystem = entity.IsSystem,
                    IsDeleted = entity.IsDeleted
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(ComponentTypeForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var componentType = DbGetOneById(entity.Id);

                if (entity.SystemName.Changed())
                    componentType.SystemName = entity.SystemName.Get();

                if (entity.DisplayName.Changed())
                    componentType.DisplayName = entity.DisplayName.Get();

                if (entity.IsDeleted.Changed())
                    componentType.IsDeleted = entity.IsDeleted.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public ComponentTypeForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public ComponentTypeForRead GetOneOrNullById(Guid id)
        {
            return DbToEntity(DbGetOneOrNullById(id));
        }

        public ComponentTypeForRead GetOneOrNullBySystemName(string systemName)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.ComponentTypes.AsNoTracking()
                    .FirstOrDefault(t => !t.IsDeleted && t.SystemName.ToLower() == systemName.ToLower()));
            }
        }

        public ComponentTypeForRead[] GetMany(Guid[] ids)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.ComponentTypes.AsNoTracking()
                    .Where(t => ids.Contains(t.Id))
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public int GetNonSystemCount()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.ComponentTypes.AsNoTracking().Count(t => !t.IsSystem && !t.IsDeleted);
            }
        }

        public ComponentTypeForRead[] Filter(string search, int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.ComponentTypes.AsNoTracking()
                    .Where(t => !t.IsDeleted);

                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    query = query.Where(t => t.SystemName.ToLower().Contains(search) ||
                                             t.DisplayName.ToLower().Contains(search) ||
                                             t.Id.ToString().ToLower().Equals(search));
                }

                query = query.OrderBy(t => t.DisplayName).Take(maxCount);

                return query
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        private DbComponentType DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.ComponentTypes.Find(id);
            }
        }

        private DbComponentType DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Тип компонента {id} не найден");

            return result;
        }

        private ComponentTypeForRead DbToEntity(DbComponentType entity)
        {
            if (entity == null)
                return null;

            return new ComponentTypeForRead(entity.Id, entity.DisplayName, entity.SystemName, entity.IsSystem, entity.IsDeleted);
        }
    }
}
