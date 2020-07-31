using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public class NotificationService : INotificationService
    {
        public NotificationService(IStorage storage)
        {
            _storage = storage;
        }

        private readonly IStorage _storage;

        public void UpdateLastComponentNotification(LastComponentNotificationForAdd lastComponentNotification,
            NotificationForRead notification, EventImportance importance)
        {
            lastComponentNotification.Address = notification.Address;
            lastComponentNotification.Type = notification.Type;
            lastComponentNotification.EventImportance = importance;
            lastComponentNotification.CreateDate = notification.CreationDate;
            lastComponentNotification.EventId = notification.EventId;
            lastComponentNotification.NotificationId = notification.Id;
        }
    }
}
