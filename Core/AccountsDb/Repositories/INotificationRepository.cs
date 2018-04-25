using System;
using System.Linq;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Репозиторий для работы с уведомлениями
    /// </summary>
    public interface INotificationRepository
    {
        Notification Add(Notification entity);

        Notification Find(Guid id, Guid componentId);

        IQueryable<Notification> QueryAllByComponent(Guid componentId);

        IQueryable<Notification> GetForSend(NotificationType notificationType);

        IQueryable<Notification> QueryAllByComponentId(Guid[] componentId);

        IQueryable<Notification> QueryAllForGui(Guid? componentId, DateTime? fromDate, DateTime? toDate, EventCategory? category, NotificationType? channel, NotificationStatus? status, Guid? userId);

        IQueryable<Notification> QueryAll();

        void DeleteBySubscriptionId(Guid subscriptionId);
    }
}
