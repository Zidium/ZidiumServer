using System;
using System.Data.Entity;
using System.Linq;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class ComponentRepository : IComponentRepository
    {
        public ComponentRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(ComponentForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.Components.Add(new DbComponent()
                {
                    Id = entity.Id,
                    ChildComponentsStatusId = entity.ChildComponentsStatusId,
                    ComponentTypeId = entity.ComponentTypeId,
                    CreatedDate = entity.CreatedDate,
                    DisableComment = entity.DisableComment,
                    DisableToDate = entity.DisableToDate,
                    DisplayName = entity.DisplayName,
                    Enable = entity.Enable,
                    EventsStatusId = entity.EventsStatusId,
                    ExternalStatusId = entity.ExternalStatusId,
                    InternalStatusId = entity.InternalStatusId,
                    IsDeleted = entity.IsDeleted,
                    MetricsStatusId = entity.MetricsStatusId,
                    ParentEnable = entity.ParentEnable,
                    ParentId = entity.ParentId,
                    SystemName = entity.SystemName,
                    UnitTestsStatusId = entity.UnitTestsStatusId,
                    Version = entity.Version
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(ComponentForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var component = DbGetOneById(entity.Id);

                if (entity.DisplayName.Changed())
                    component.DisplayName = entity.DisplayName.Get();

                if (entity.SystemName.Changed())
                    component.SystemName = entity.SystemName.Get();

                if (entity.ParentId.Changed())
                    component.ParentId = entity.ParentId.Get();

                if (entity.ComponentTypeId.Changed())
                    component.ComponentTypeId = entity.ComponentTypeId.Get();

                if (entity.Version.Changed())
                    component.Version = entity.Version.Get();

                if (entity.Enable.Changed())
                    component.Enable = entity.Enable.Get();

                if (entity.DisableToDate.Changed())
                    component.DisableToDate = entity.DisableToDate.Get();

                if (entity.DisableComment.Changed())
                    component.DisableComment = entity.DisableComment.Get();

                if (entity.ParentEnable.Changed())
                    component.ParentEnable = entity.ParentEnable.Get();

                if (entity.IsDeleted.Changed())
                    component.IsDeleted = entity.IsDeleted.Get();

                if (entity.InternalStatusId.Changed())
                    component.InternalStatusId = entity.InternalStatusId.Get();

                if (entity.ExternalStatusId.Changed())
                    component.ExternalStatusId = entity.ExternalStatusId.Get();

                if (entity.UnitTestsStatusId.Changed())
                    component.UnitTestsStatusId = entity.UnitTestsStatusId.Get();

                if (entity.EventsStatusId.Changed())
                    component.EventsStatusId = entity.EventsStatusId.Get();

                if (entity.MetricsStatusId.Changed())
                    component.MetricsStatusId = entity.MetricsStatusId.Get();

                if (entity.ChildComponentsStatusId.Changed())
                    component.ChildComponentsStatusId = entity.ChildComponentsStatusId.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(ComponentForUpdate[] entities)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                foreach (var entity in entities)
                {
                    var component = DbGetOneById(entity.Id);

                    if (entity.DisplayName.Changed())
                        component.DisplayName = entity.DisplayName.Get();

                    if (entity.SystemName.Changed())
                        component.SystemName = entity.SystemName.Get();

                    if (entity.ParentId.Changed())
                        component.ParentId = entity.ParentId.Get();

                    if (entity.ComponentTypeId.Changed())
                        component.ComponentTypeId = entity.ComponentTypeId.Get();

                    if (entity.Version.Changed())
                        component.Version = entity.Version.Get();

                    if (entity.Enable.Changed())
                        component.Enable = entity.Enable.Get();

                    if (entity.DisableToDate.Changed())
                        component.DisableToDate = entity.DisableToDate.Get();

                    if (entity.DisableComment.Changed())
                        component.DisableComment = entity.DisableComment.Get();

                    if (entity.ParentEnable.Changed())
                        component.ParentEnable = entity.ParentEnable.Get();

                    if (entity.IsDeleted.Changed())
                        component.IsDeleted = entity.IsDeleted.Get();

                    if (entity.InternalStatusId.Changed())
                        component.InternalStatusId = entity.InternalStatusId.Get();

                    if (entity.ExternalStatusId.Changed())
                        component.ExternalStatusId = entity.ExternalStatusId.Get();

                    if (entity.UnitTestsStatusId.Changed())
                        component.UnitTestsStatusId = entity.UnitTestsStatusId.Get();

                    if (entity.EventsStatusId.Changed())
                        component.EventsStatusId = entity.EventsStatusId.Get();

                    if (entity.MetricsStatusId.Changed())
                        component.MetricsStatusId = entity.MetricsStatusId.Get();

                    if (entity.ChildComponentsStatusId.Changed())
                        component.ChildComponentsStatusId = entity.ChildComponentsStatusId.Get();
                }

                contextWrapper.Context.SaveChanges();
            }
        }

        public ComponentForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public ComponentForRead GetOneOrNullById(Guid id)
        {
            return DbToEntity(DbGetOneOrNullById(id));
        }

        public ComponentForRead[] GetMany(Guid[] ids)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Components.AsNoTracking()
                    .Where(t => ids.Contains(t.Id))
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public ComponentForRead GetRoot()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.Components.AsNoTracking().First(t => t.ParentId == null));
            }
        }

        public ComponentForRead[] GetChilds(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Components.AsNoTracking()
                    .Where(t => t.ParentId == id && !t.IsDeleted)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public ComponentForRead GetChild(Guid parentId, string systemName)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(
                    contextWrapper.Context.Components.AsNoTracking()
                        .FirstOrDefault(t =>
                            t.ParentId == parentId && !t.IsDeleted && t.SystemName.ToLower() == systemName.ToLower()));
            }
        }

        public Guid[] GetNotActualEventsStatusIds(DateTime now, int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Components.AsNoTracking()
                    .Where(t => t.EventsStatus.ActualDate < now && !t.IsDeleted)
                    .OrderBy(x => x.EventsStatus.ActualDate)
                    .Take(maxCount)
                    .Select(t => t.EventsStatusId)
                    .ToArray();
            }
        }

        public ComponentGetAllIdsWithParentsInfo[] GetAllIdsWithParents()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Components.AsNoTracking()
                    .Where(t => !t.IsDeleted)
                    .Select(t => new ComponentGetAllIdsWithParentsInfo()
                    {
                        Id = t.Id,
                        ParentId = t.ParentId
                    })
                    .ToArray();
            }
        }

        public ComponentForRead[] GetByComponentTypeId(Guid componentTypeId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Components.AsNoTracking()
                    .Where(t => t.ComponentTypeId == componentTypeId && !t.IsDeleted)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public int GetCount()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Components.AsNoTracking().Count(t => !t.IsDeleted);
            }
        }

        public Guid[] GetAllIds()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Components.AsNoTracking()
                    .Where(t => !t.IsDeleted)
                    .Select(t => t.Id)
                    .ToArray();
            }
        }

        public ComponentGetForNotificationsInfo[] GetForNotifications(Guid? componentId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.Components.AsNoTracking()
                    .Include(t => t.LastNotifications)
                    .Where(t => !t.IsDeleted);

                if (componentId.HasValue)
                    query = query.Where(t => t.Id == componentId.Value);

                return query.Select(t => new ComponentGetForNotificationsInfo()
                {
                    Id = t.Id,
                    ComponentTypeId = t.ComponentTypeId,
                    LastComponentNotifications = t.LastNotifications
                        .Select(x => new ComponentGetForNotificationsInfo.LastComponentNotificationInfo()
                        {
                            Id = x.Id,
                            CreateDate = x.CreateDate,
                            Type = x.Type,
                            EventId = x.EventId,
                            EventImportance = x.EventImportance,
                            Address = x.Address
                        }).ToList()
                }).ToArray();
            }
        }

        private DbComponent DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Components.Find(id);
            }
        }

        private DbComponent DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Компонент {id} не найден");

            return result;
        }

        private ComponentForRead DbToEntity(DbComponent entity)
        {
            if (entity == null)
                return null;

            return new ComponentForRead(entity.Id, entity.CreatedDate, entity.DisplayName, entity.SystemName, entity.ParentId,
                entity.ComponentTypeId, entity.InternalStatusId, entity.ExternalStatusId, entity.UnitTestsStatusId,
                entity.EventsStatusId, entity.MetricsStatusId, entity.ChildComponentsStatusId, entity.Version,
                entity.IsDeleted, entity.Enable, entity.DisableToDate, entity.DisableComment, entity.ParentEnable);
        }
    }
}
