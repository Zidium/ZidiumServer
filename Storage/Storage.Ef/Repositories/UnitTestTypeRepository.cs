using System;
using System.Linq;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class UnitTestTypeRepository : IUnitTestTypeRepository
    {
        public UnitTestTypeRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(UnitTestTypeForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.UnitTestTypes.Add(new DbUnitTestType()
                {
                    Id = entity.Id,
                    DisplayName = entity.DisplayName,
                    CreateDate = entity.CreateDate,
                    SystemName = entity.SystemName,
                    IsDeleted = entity.IsDeleted,
                    IsSystem = entity.IsSystem,
                    NoSignalColor = entity.NoSignalColor,
                    ActualTimeSecs = entity.ActualTimeSecs
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(UnitTestTypeForUpdate[] entities)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                foreach (var entity in entities)
                {
                    var unittestType = DbGetOneById(entity.Id);

                    if (entity.SystemName.Changed())
                        unittestType.SystemName = entity.SystemName.Get();

                    if (entity.DisplayName.Changed())
                        unittestType.DisplayName = entity.DisplayName.Get();

                    if (entity.IsDeleted.Changed())
                        unittestType.IsDeleted = entity.IsDeleted.Get();

                    if (entity.NoSignalColor.Changed())
                        unittestType.NoSignalColor = entity.NoSignalColor.Get();

                    if (entity.ActualTimeSecs.Changed())
                        unittestType.ActualTimeSecs = entity.ActualTimeSecs.Get();
                }

                contextWrapper.Context.SaveChanges();
            }
        }

        public UnitTestTypeForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public UnitTestTypeForRead GetOneOrNullById(Guid id)
        {
            return DbToEntity(DbGetOneOrNullById(id));
        }

        public UnitTestTypeForRead GetOneOrNullBySystemName(string systemName)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.UnitTestTypes.AsNoTracking()
                    .FirstOrDefault(t => !t.IsDeleted && t.SystemName.ToLower() == systemName.ToLower()));
            }
        }

        public UnitTestTypeForRead[] GetMany(Guid[] ids)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UnitTestTypes.AsNoTracking()
                    .Where(t => !t.IsDeleted && ids.Contains(t.Id))
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public UnitTestTypeForRead[] GetAllWithDeleted()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UnitTestTypes.AsNoTracking()
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public int GetNonSystemCount()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UnitTestTypes.Count(t => !t.IsDeleted && !t.IsSystem);
            }
        }

        public UnitTestTypeForRead[] Filter(string search, int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.UnitTestTypes.AsNoTracking()
                    .Where(t => !t.IsDeleted);

                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    query = query.Where(t => t.Id.ToString().ToLower() == search || t.SystemName.ToLower().Contains(search) || t.DisplayName.ToLower().Contains(search));
                }

                query = query
                    .OrderBy(t => t.IsSystem)
                    .ThenBy(t => t.DisplayName)
                    .Take(maxCount);

                return query
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        private DbUnitTestType DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UnitTestTypes.Find(id);
            }
        }

        private DbUnitTestType DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Тип проверки {id} не найден");

            return result;
        }

        private UnitTestTypeForRead DbToEntity(DbUnitTestType entity)
        {
            if (entity == null)
                return null;

            return new UnitTestTypeForRead(entity.Id, entity.SystemName, entity.DisplayName, entity.CreateDate,
                entity.IsDeleted, entity.IsSystem, entity.NoSignalColor, entity.ActualTimeSecs);
        }
    }
}
