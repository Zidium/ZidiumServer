
namespace Zidium.Core.AccountsDb
{
    public enum NotificationStatus
    {
        /// <summary>
        /// Ожидает отправки
        /// </summary>
        InQueue = 1,

        /// <summary>
        /// Успешно отправлено
        /// </summary>
        Sended = 2,

        /// <summary>
        /// Ошибка отправки
        /// </summary>
        Error = 3
    }
}
