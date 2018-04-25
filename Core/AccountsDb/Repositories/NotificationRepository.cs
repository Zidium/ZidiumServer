using System;
using System.Data.Entity;
using System.Linq;
using Zidium.Core.Api;
using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Репозиторий для работы с уведомлениями
    /// </summary>    
    public class NotificationRepository : INotificationRepository
    {
        protected AccountDbContext Context;

        public NotificationRepository(AccountDbContext context)
        {
            Context = context;
        }

        public IQueryable<Notification> QueryAllByComponentId(Guid[] componentId)
        {
            return Context.Notifications.Include("Event").Where(t => componentId.Contains(t.Event.OwnerId));
        }

        public Notification Add(Notification entity)
        {
            Context.Notifications.Add(entity);
            return entity;
        }

        public IQueryable<Notification> QueryAllForGui(Guid? componentId, DateTime? fromDate, DateTime? toDate, EventCategory? category, NotificationType? channel, NotificationStatus? status, Guid? userId)
        {
            var query = QueryAll().Include("Event");
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
            return query;
        }

        public IQueryable<Notification> QueryAll()
        {
            return Context.Notifications;
        }

        public void DeleteBySubscriptionId(Guid subscriptionId)
        {
            using (var connection = Context.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;

                    var query = @"
                        DELETE FROM [dbo].[LastComponentNotifications]
                        WHERE NotificationId IN (
                        SELECT Id 
                        FROM [dbo].[Notifications]
                        WHERE SubscriptionId = @SubscriptionId)

                        DELETE FROM [dbo].[NotificationsHttp]
                        WHERE NotificationId IN (
                        SELECT Id 
                        FROM [dbo].[Notifications]
                        WHERE SubscriptionId = @SubscriptionId)

                        DELETE FROM [dbo].[Notifications]
                        WHERE SubscriptionId = @SubscriptionId";

                    command.CommandText = query;

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@SubscriptionId";
                    parameter.Value = subscriptionId;
                    command.Parameters.Add(parameter);

                    SqlCommandHelper.ExecuteNonQuery(command);
                }
            }
        }

        public IQueryable<Notification> GetForSend(NotificationType notificationType)
        {
            return Context
                .Notifications
                .Where(x =>
                    x.Type == notificationType &&
                    x.Status == NotificationStatus.InQueue);
        }

        public Notification Find(Guid id, Guid componentId)
        {
            var result = Context.Notifications.Find(id);
            if (result == null)
                throw new ObjectNotFoundException(id, Naming.Notification);
            if (result.Event.OwnerId != componentId)
                throw new AccessDeniedException(id, Naming.Notification);
            return result;
        }

        public IQueryable<Notification> QueryAllByComponent(Guid componentId)
        {
            return Context.Notifications.Where(t => t.Event.OwnerId == componentId);
        }

        public void Remove(Notification entity)
        {
            Context.Notifications.Remove(entity);
            Context.SaveChanges();
        }

        public void Remove(Guid id, Guid componentId)
        {
            var entity = Find(id, componentId);
            Remove(entity);
        }
    }
}
