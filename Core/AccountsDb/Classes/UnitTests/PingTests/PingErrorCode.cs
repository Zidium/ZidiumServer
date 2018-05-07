namespace Zidium.Core.AccountsDb
{
    public enum PingErrorCode
    {
        /// <summary>
        /// Нет ошибок (все хорошо)
        /// </summary>
        Success = 1,

        /// <summary>
        /// Не удалось получить IP доменного имени
        /// </summary>
        UnknownDomain = 2,

        /// <summary>
        /// Превышение таймаута
        /// </summary>
        Timeout = 3,
    }
}
