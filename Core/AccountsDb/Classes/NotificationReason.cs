namespace Zidium.Core.AccountsDb
{
    public enum NotificationReason
    {
        /// <summary>
        /// Неизвестно
        /// Используется, чтобы отлавливать ошибки инициализации по умолчанию
        /// Данное значение в БД быть не должно
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Уведомление о новом важном статусе
        /// </summary>
        NewImportanceStatus = 1,

        /// <summary>
        /// Напоминание о текущем статусе
        /// </summary>
        Reminder = 2,

        /// <summary>
        /// Уведомление о том, что компоненту стало лучше
        /// </summary>
        BetterStatus = 3
    }
}
