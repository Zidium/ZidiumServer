using System;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class LastComponentNotificationRepository : ILastComponentNotificationRepository
    {
        public LastComponentNotificationRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(LastComponentNotificationForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.LastComponentNotifications.Add(new DbLastComponentNotification()
                {
                    Id = entity.Id,
                    CreateDate = entity.CreateDate,
                    ComponentId = entity.ComponentId,
                    Address = entity.Address,
                    EventId = entity.EventId,
                    EventImportance = entity.EventImportance,
                    NotificationId = entity.NotificationId,
                    Type = entity.Type
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(LastComponentNotificationForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var lastNotification = DbGetOneById(entity.Id);

                if (entity.EventImportance.Changed())
                    lastNotification.EventImportance = entity.EventImportance.Get();

                if (entity.CreateDate.Changed())
                    lastNotification.CreateDate = entity.CreateDate.Get();

                if (entity.EventId.Changed())
                    lastNotification.EventId = entity.EventId.Get();

                if (entity.NotificationId.Changed())
                    lastNotification.NotificationId = entity.NotificationId.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public LastComponentNotificationForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        private DbLastComponentNotification DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.LastComponentNotifications.Find(id);
            }
        }

        private DbLastComponentNotification DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Последнее уведомление {id} не найдено");

            return result;
        }

        private LastComponentNotificationForRead DbToEntity(DbLastComponentNotification entity)
        {
            if (entity == null)
                return null;

            return new LastComponentNotificationForRead(entity.Id, entity.ComponentId, entity.Address, entity.Type,
                entity.EventImportance, entity.CreateDate, entity.EventId, entity.NotificationId);
        }
    }
}
