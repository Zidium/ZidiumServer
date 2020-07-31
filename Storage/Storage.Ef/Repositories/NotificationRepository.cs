using System;
using System.Data.Entity;
using System.Linq;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class NotificationRepository : INotificationRepository
    {
        public NotificationRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(NotificationForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.Notifications.Add(new DbNotification()
                {
                    Id = entity.Id,
                    Status = entity.Status,
                    UserId = entity.UserId,
                    Type = entity.Type,
                    Address = entity.Address,
                    CreationDate = entity.CreationDate,
                    EventId = entity.EventId,
                    Reason = entity.Reason,
                    SendDate = entity.SendDate,
                    SendEmailCommandId = entity.SendEmailCommandId,
                    SendError = entity.SendError,
                    SendMessageCommandId = entity.SendMessageCommandId,
                    SubscriptionId = entity.SubscriptionId
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(NotificationForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var notification = DbGetOneById(entity.Id);

                if (entity.Status.Changed())
                    notification.Status = entity.Status.Get();

                if (entity.SendError.Changed())
                    notification.SendError = entity.SendError.Get();

                if (entity.SendDate.Changed())
                    notification.SendDate = entity.SendDate.Get();

                if (entity.SendEmailCommandId.Changed())
                    notification.SendEmailCommandId = entity.SendEmailCommandId.Get();

                if (entity.SendMessageCommandId.Changed())
                    notification.SendMessageCommandId = entity.SendMessageCommandId.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public void DeleteBySubscriptionId(Guid subscriptionId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                using (var connection = contextWrapper.Context.CreateConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandTimeout = 0;

                        var query = $@"
                        DELETE FROM {contextWrapper.Context.FormatTableName("LastComponentNotifications")}
                        WHERE {contextWrapper.Context.FormatColumnName("NotificationId")} IN (
                        SELECT {contextWrapper.Context.FormatColumnName("Id")} 
                        FROM {contextWrapper.Context.FormatTableName("Notifications")}
                        WHERE {contextWrapper.Context.FormatColumnName("SubscriptionId")} = @SubscriptionId)

                        DELETE {contextWrapper.Context.FormatTableName("NotificationsHttp")}
                        WHERE {contextWrapper.Context.FormatColumnName("NotificationId")} IN (
                        SELECT {contextWrapper.Context.FormatColumnName("Id")} 
                        FROM {contextWrapper.Context.FormatTableName("Notifications")}
                        WHERE {contextWrapper.Context.FormatColumnName("SubscriptionId")} = @SubscriptionId)

                        DELETE FROM {contextWrapper.Context.FormatTableName("Notifications")}
                        WHERE {contextWrapper.Context.FormatColumnName("SubscriptionId")} = @SubscriptionId";

                        command.CommandText = query;

                        var parameter = command.CreateParameter();
                        parameter.ParameterName = "@SubscriptionId";
                        parameter.Value = subscriptionId;
                        command.Parameters.Add(parameter);

                        SqlCommandHelper.ExecuteNonQuery(command);
                    }
                }
            }
        }

        public NotificationForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public NotificationForRead GetOneOrNullById(Guid id)
        {
            return DbToEntity(DbGetOneOrNullById(id));
        }

        public NotificationForRead[] GetForSend(SubscriptionChannel[] channels, Guid? componentId, int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.Notifications.AsNoTracking()
                    .Where(x => x.Status == NotificationStatus.InQueue &&
                                channels.Contains(x.Type));

                if (componentId.HasValue)
                    query = query.Where(t => t.Event.OwnerId == componentId.Value);

                return query.OrderBy(t => t.CreationDate)
                    .Take(maxCount)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public NotificationForRead[] Filter(
            Guid? componentId,
            DateTime? fromDate,
            DateTime? toDate,
            EventCategory? category,
            SubscriptionChannel? channel,
            NotificationStatus? status,
            Guid? userId,
            int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.Notifications.AsNoTracking().AsQueryable()
                    .Include(t => t.Event)
                    .Include(t => t.User)
                    .Include(t => t.Event.EventType);

                if (componentId.HasValue)
                    query = query.Where(t => t.Event.OwnerId == componentId.Value);

                if (fromDate.HasValue)
                    query = query.Where(t => t.CreationDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(t => t.CreationDate <= toDate.Value);

                if (category.HasValue)
                    query = query.Where(t => t.Event.Category == category.Value);

                if (channel.HasValue)
                    query = query.Where(t => t.Type == channel.Value);

                if (status.HasValue)
                    query = query.Where(t => t.Status == status.Value);

                if (userId.HasValue)
                    query = query.Where(t => t.UserId == userId.Value);

                return query.OrderBy(t => t.CreationDate)
                    .Take(maxCount)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        private DbNotification DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Notifications.Find(id);
            }
        }

        private DbNotification DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Уведомление {id} не найдено");

            return result;
        }

        private NotificationForRead DbToEntity(DbNotification entity)
        {
            if (entity == null)
                return null;

            return new NotificationForRead(entity.Id, entity.EventId, entity.UserId, entity.Type, entity.Status,
                entity.SendError, entity.CreationDate, entity.SendDate, entity.SubscriptionId, entity.Reason,
                entity.Address, entity.SendEmailCommandId, entity.SendMessageCommandId);
        }
    }
}
