namespace Zidium.Api
{
    /// <summary>
    /// Уровень важности события
    /// </summary>
    public enum EventImportance
    {
        /// <summary>
        /// Неизвестно (не меняет статус компонента)
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Информационное событие
        /// </summary>
        Success = 50,

        /// <summary>
        /// Событие, требующее внимания
        /// </summary>
        Warning = 100,

        /// <summary>
        /// Опасное событие, необходимо принять меры
        /// </summary>
        Alarm = 150
    }
}
