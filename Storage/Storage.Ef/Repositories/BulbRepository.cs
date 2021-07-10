using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zidium.Api.Dto;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class BulbRepository : IBulbRepository
    {
        public BulbRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(BulbForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.Bulbs.Add(new DbBulb()
                {
                    Id = entity.Id,
                    UnitTestId = entity.UnitTestId,
                    CreateDate = entity.CreateDate,
                    ComponentId = entity.ComponentId,
                    Status = entity.Status,
                    EventCategory = entity.EventCategory,
                    ActualDate = entity.ActualDate,
                    Count = entity.Count,
                    EndDate = entity.EndDate,
                    FirstEventId = entity.FirstEventId,
                    HasSignal = entity.HasSignal,
                    IsDeleted = entity.IsDeleted,
                    LastChildBulbId = entity.LastChildBulbId,
                    LastEventId = entity.LastEventId,
                    Message = entity.Message,
                    MetricId = entity.MetricId,
                    PreviousStatus = entity.PreviousStatus,
                    StartDate = entity.StartDate,
                    StatusEventId = entity.StatusEventId,
                    UpTimeLengthMs = entity.UpTimeLengthMs,
                    UpTimeStartDate = entity.UpTimeStartDate,
                    UpTimeSuccessMs = entity.UpTimeSuccessMs
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(BulbForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var bulb = DbGetOneById(entity.Id);

                if (entity.ComponentId.Changed())
                    bulb.ComponentId = entity.ComponentId.Get();

                if (entity.MetricId.Changed())
                    bulb.MetricId = entity.MetricId.Get();

                if (entity.UnitTestId.Changed())
                    bulb.UnitTestId = entity.UnitTestId.Get();

                if (entity.StatusEventId.Changed())
                    bulb.StatusEventId = entity.StatusEventId.Get();

                if (entity.IsDeleted.Changed())
                    bulb.IsDeleted = entity.IsDeleted.Get();

                if (entity.LastChildBulbId.Changed())
                    bulb.LastChildBulbId = entity.LastChildBulbId.Get();

                if (entity.LastEventId.Changed())
                    bulb.LastEventId = entity.LastEventId.Get();

                if (entity.Message.Changed())
                    bulb.Message = entity.Message.Get();

                if (entity.Status.Changed())
                    bulb.Status = entity.Status.Get();

                if (entity.PreviousStatus.Changed())
                    bulb.PreviousStatus = entity.PreviousStatus.Get();

                if (entity.StartDate.Changed())
                    bulb.StartDate = entity.StartDate.Get();

                if (entity.EndDate.Changed())
                    bulb.EndDate = entity.EndDate.Get();

                if (entity.Count.Changed())
                    bulb.Count = entity.Count.Get();

                if (entity.ActualDate.Changed())
                    bulb.ActualDate = entity.ActualDate.Get();

                if (entity.FirstEventId.Changed())
                    bulb.FirstEventId = entity.FirstEventId.Get();

                if (entity.HasSignal.Changed())
                    bulb.HasSignal = entity.HasSignal.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(BulbForUpdate[] entities)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                foreach (var entity in entities)
                {
                    var bulb = DbGetOneById(entity.Id);

                    if (entity.ComponentId.Changed())
                        bulb.ComponentId = entity.ComponentId.Get();

                    if (entity.MetricId.Changed())
                        bulb.MetricId = entity.MetricId.Get();

                    if (entity.UnitTestId.Changed())
                        bulb.UnitTestId = entity.UnitTestId.Get();

                    if (entity.StatusEventId.Changed())
                        bulb.StatusEventId = entity.StatusEventId.Get();

                    if (entity.IsDeleted.Changed())
                        bulb.IsDeleted = entity.IsDeleted.Get();

                    if (entity.LastChildBulbId.Changed())
                        bulb.LastChildBulbId = entity.LastChildBulbId.Get();

                    if (entity.LastEventId.Changed())
                        bulb.LastEventId = entity.LastEventId.Get();

                    if (entity.Message.Changed())
                        bulb.Message = entity.Message.Get();

                    if (entity.Status.Changed())
                        bulb.Status = entity.Status.Get();

                    if (entity.PreviousStatus.Changed())
                        bulb.PreviousStatus = entity.PreviousStatus.Get();

                    if (entity.StartDate.Changed())
                        bulb.StartDate = entity.StartDate.Get();

                    if (entity.EndDate.Changed())
                        bulb.EndDate = entity.EndDate.Get();

                    if (entity.Count.Changed())
                        bulb.Count = entity.Count.Get();

                    if (entity.ActualDate.Changed())
                        bulb.ActualDate = entity.ActualDate.Get();

                    if (entity.FirstEventId.Changed())
                        bulb.FirstEventId = entity.FirstEventId.Get();

                    if (entity.HasSignal.Changed())
                        bulb.HasSignal = entity.HasSignal.Get();
                }

                contextWrapper.Context.SaveChanges();
            }
        }

        public BulbForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public BulbForRead GetOneOrNullById(Guid id)
        {
            return DbToEntity(DbGetOneOrNullById(id));
        }

        public BulbForRead[] GetMany(Guid[] ids)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Bulbs.AsNoTracking()
                    .Where(t => ids.Contains(t.Id))
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public BulbGetForNotificationsInfo[] GetForNotifications()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Bulbs.AsNoTracking()
                    .Where(x => x.EventCategory == EventCategory.ComponentExternalStatus
                                && x.ComponentId != null
                                && !x.IsDeleted
                                && !x.Component.IsDeleted)
                    .Select(t => new BulbGetForNotificationsInfo()
                    {
                        Id = t.Id,
                        Status = t.Status,
                        ComponentId = t.ComponentId.Value,
                        ComponentTypeId = t.Component.ComponentTypeId,
                        ActualDate = t.ActualDate,
                        StartDate = t.StartDate,
                        Category = t.EventCategory,
                        PreviousStatus = t.PreviousStatus,
                        StatusEventId = t.StatusEventId,
                        LastComponentNotifications = t.Component.LastNotifications
                            .Select(x => new ComponentGetForNotificationsInfo.LastComponentNotificationInfo()
                            {
                                Id = x.Id,
                                CreateDate = x.CreateDate,
                                Type = x.Type,
                                EventImportance = x.EventImportance,
                                EventId = x.EventId,
                                Address = x.Address
                            }).ToList()
                    })
                    .ToArray();
            }
        }

        private DbBulb DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Bulbs.Find(id);
            }
        }

        private DbBulb DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Статус {id} не найден");

            return result;
        }

        public static BulbForRead DbToEntity(DbBulb entity)
        {
            if (entity == null)
                return null;

            return new BulbForRead(entity.Id, entity.ComponentId, entity.UnitTestId, entity.MetricId, entity.EventCategory,
                entity.Status, entity.PreviousStatus, entity.CreateDate, entity.StartDate, entity.EndDate, entity.Count,
                entity.ActualDate, entity.Message, entity.FirstEventId, entity.LastEventId, entity.LastChildBulbId,
                entity.StatusEventId, entity.UpTimeStartDate, entity.UpTimeLengthMs, entity.UpTimeSuccessMs, entity.HasSignal,
                entity.IsDeleted);
        }
    }
}
