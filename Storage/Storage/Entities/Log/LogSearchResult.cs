namespace Zidium.Storage
{
    /// <summary>
    /// Результат поиска по тексту
    /// </summary>
    public class LogSearchResult
    {
        /// <summary>
        /// Удалось найти?
        /// </summary>
        public bool Found { get; set; }

        /// <summary>
        /// Запись лога, которую удалось найти, или с которой можно продолжить поиск
        /// </summary>
        public LogForRead Record { get; set; }

    }
}
