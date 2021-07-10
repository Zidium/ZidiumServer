using System;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class NotificationHttpRepository : INotificationHttpRepository
    {
        public NotificationHttpRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(NotificationHttpForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.NotificationsHttp.Add(new DbNotificationHttp()
                {
                    NotificationId = entity.NotificationId,
                    Json = entity.Json
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(NotificationHttpForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var notificationHttp = DbGetOneById(entity.NotificationId);

                if (entity.Json.Changed())
                    notificationHttp.Json = entity.Json.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public NotificationHttpForRead GetByNotificationId(Guid notificationId)
        {
            return DbToEntity(DbGetOneById(notificationId));
        }

        private DbNotificationHttp DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.NotificationsHttp.Find(id);
            }
        }

        private DbNotificationHttp DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Http-уведомление {id} не найдено");

            return result;
        }

        private NotificationHttpForRead DbToEntity(DbNotificationHttp entity)
        {
            if (entity == null)
                return null;

            return new NotificationHttpForRead(entity.NotificationId, entity.Json);
        }
    }
}
