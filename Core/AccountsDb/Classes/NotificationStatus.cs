
namespace Zidium.Core.AccountsDb
{
    public enum NotificationStatus
    {
        /// <summary>
        /// Ожидает отправки
        /// </summary>
        InQueue = 1,

        /// <summary>
        /// Создана команда на отправку
        /// </summary>
        Processed = 2,

        /// <summary>
        /// Ошибка отправки
        /// </summary>
        Error = 3,

        /// <summary>
        /// Успешно отправлено
        /// </summary>
        Sent = 4
    }
}
