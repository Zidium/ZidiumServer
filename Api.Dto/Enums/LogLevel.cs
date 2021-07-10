namespace Zidium.Api.Dto
{
    /// <summary>
    /// Уровень важности сообщения (события)
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Используется для трассировки
        /// </summary>
        Trace = 10,

        /// <summary>
        /// Используется для отладочных целей
        /// </summary>
        Debug = 20,

        /// <summary>
        /// Обычная информация
        /// </summary>
        Info = 30,

        /// <summary>
        /// Предупреждение
        /// </summary>
        Warning = 40,

        /// <summary>
        /// Ошибка (сбой)
        /// </summary>
        Error = 50,

        /// <summary>
        /// Фотальная ошибка (сбой)
        /// </summary>
        Fatal = 60
    }
}
