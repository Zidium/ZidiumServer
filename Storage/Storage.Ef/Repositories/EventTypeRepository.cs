using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zidium.Api.Dto;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class EventTypeRepository : IEventTypeRepository
    {
        public EventTypeRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(EventTypeForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.EventTypes.Add(new DbEventType()
                {
                    Id = entity.Id,
                    DisplayName = entity.DisplayName,
                    CreateDate = entity.CreateDate,
                    SystemName = entity.SystemName,
                    Category = entity.Category,
                    IsDeleted = entity.IsDeleted,
                    IsSystem = entity.IsSystem,
                    DefectId = entity.DefectId,
                    JoinIntervalSeconds = entity.JoinIntervalSeconds,
                    ImportanceForNew = entity.ImportanceForNew,
                    ImportanceForOld = entity.ImportanceForOld,
                    Code = entity.Code,
                    OldVersion = entity.OldVersion
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(EventTypeForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var eventType = DbGetOneById(entity.Id);

                if (entity.Category.Changed())
                    eventType.Category = entity.Category.Get();

                if (entity.DisplayName.Changed())
                    eventType.DisplayName = entity.DisplayName.Get();

                if (entity.SystemName.Changed())
                    eventType.SystemName = entity.SystemName.Get();

                if (entity.Code.Changed())
                    eventType.Code = entity.Code.Get();

                if (entity.JoinIntervalSeconds.Changed())
                    eventType.JoinIntervalSeconds = entity.JoinIntervalSeconds.Get();

                if (entity.OldVersion.Changed())
                    eventType.OldVersion = entity.OldVersion.Get();

                if (entity.ImportanceForOld.Changed())
                    eventType.ImportanceForOld = entity.ImportanceForOld.Get();

                if (entity.ImportanceForNew.Changed())
                    eventType.ImportanceForNew = entity.ImportanceForNew.Get();

                if (entity.IsDeleted.Changed())
                    eventType.IsDeleted = entity.IsDeleted.Get();

                if (entity.DefectId.Changed())
                    eventType.DefectId = entity.DefectId.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public EventTypeForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public EventTypeForRead GetOneOrNullById(Guid id)
        {
            return DbToEntity(DbGetOneOrNullById(id));
        }

        public EventTypeForRead GetOneOrNullByCategoryAndName(EventCategory category, string systemName)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.EventTypes.AsNoTracking()
                    .FirstOrDefault(t => t.Category == category && !t.IsDeleted && t.SystemName.ToLower() == systemName.ToLower()));
            }
        }

        public EventTypeForRead GetOneOrNullBySystemName(string systemName)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.EventTypes.AsNoTracking()
                    .FirstOrDefault(t => !t.IsDeleted && t.SystemName.ToLower() == systemName.ToLower()));
            }
        }

        public EventTypeForRead[] GetMany(Guid[] ids)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.EventTypes.AsNoTracking()
                    .Where(t => ids.Contains(t.Id))
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public EventTypeForRead[] Filter(
            EventImportance? importance,
            EventCategory? category,
            string search,
            int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.EventTypes.AsNoTracking()
                    .Where(t => !t.IsDeleted);

                if (importance.HasValue)
                    query = query.Where(t => t.ImportanceForNew == importance.Value);

                if (category.HasValue)
                    query = query.Where(t => t.Category == category.Value);

                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    query = query.Where(t => t.SystemName.ToLower().Contains(search) ||
                                             t.DisplayName.ToLower().Contains(search));
                }

                query = query.OrderBy(t => t.DisplayName).Take(maxCount);

                return query.AsEnumerable().Select(DbToEntity).ToArray();
            }
        }

        private DbEventType DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.EventTypes.Find(id);
            }
        }

        private DbEventType DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Тип события {id} не найден");

            return result;
        }

        private EventTypeForRead DbToEntity(DbEventType entity)
        {
            if (entity == null)
                return null;

            return new EventTypeForRead(entity.Id, entity.Category, entity.DisplayName, entity.SystemName, entity.Code,
                entity.JoinIntervalSeconds, entity.IsSystem, entity.OldVersion, entity.ImportanceForOld, entity.ImportanceForNew,
                entity.IsDeleted, entity.CreateDate, entity.DefectId);
        }
    }
}
