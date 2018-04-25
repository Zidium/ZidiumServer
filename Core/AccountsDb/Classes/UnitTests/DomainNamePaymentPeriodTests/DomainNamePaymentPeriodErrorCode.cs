namespace Zidium.Core.AccountsDb
{
    public enum DomainNamePaymentPeriodErrorCode
    {
        /// <summary>
        /// Нет ошибок (все хорошо)
        /// </summary>
        Success = 1,

        /// <summary>
        /// Доменное имя свободно
        /// </summary>
        FreeDomain = 2,

        /// <summary>
        /// Проверка поддоменов не поддерживается
        /// </summary>
        SubDomain = 3,

        /// <summary>
        /// Сервис временно недоступен
        /// </summary>
        Unavailable = 4,

        /// <summary>
        /// Неизвестная ошибка
        /// </summary>
        UnknownError = 50
    }
}
