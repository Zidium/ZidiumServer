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

        Notification Find(Guid id);

        Notification Find(Guid id, Guid componentId);

        Notification GetOneOrNullById(Guid id);

        IQueryable<Notification> QueryAllByComponent(Guid componentId);

        IQueryable<Notification> GetForSend(SubscriptionChannel[] channels);

        IQueryable<Notification> QueryAllByComponentId(Guid[] componentId);

        IQueryable<Notification> QueryAllForGui(Guid? componentId, DateTime? fromDate, DateTime? toDate, EventCategory? category, SubscriptionChannel? channel, NotificationStatus? status, Guid? userId);

        IQueryable<Notification> QueryAll();

        void DeleteBySubscriptionId(Guid subscriptionId);
    }
}
