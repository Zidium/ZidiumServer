using Zidium.Api.Dto;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public interface INotificationService
    {
        void UpdateLastComponentNotification(LastComponentNotificationForAdd lastComponentNotification,
            NotificationForRead notification, EventImportance importance);
    }
}
