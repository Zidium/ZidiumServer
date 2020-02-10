namespace Zidium.Core.AccountsDb
{
    public enum VirusTotalErrorCode
    {
        /// <summary>
        /// Сайт чистый (нет проблем)
        /// </summary>
        CleanSite = 1,

        /// <summary>
        /// Обнаружены проблемы
        /// </summary>
        ProblemsDetected = 2,

        /// <summary>
        /// Нет доступа (скорее всего неверный ключ)
        /// </summary>
        ServiceForbidden = 3,

        /// <summary>
        /// Ошибка, которую вернул АПИ virus total
        /// </summary>
        VirusTotalApiError = 4,

        /// <summary>
        /// Неизвестная ошибка
        /// </summary>
        UnknownError = 100
    }
}
