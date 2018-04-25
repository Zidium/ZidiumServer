namespace Zidium.Core.Api
{
    /// <summary>
    /// Статус компонента
    /// </summary>
    public enum MonitoringStatus
    {
        /// <summary>
        /// Нет актуальных данных, по которым можно судить о состоянии компонента.
        /// Например, нет ни одного актуального события.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Отключен
        /// </summary>
        Disabled = 50,

        /// <summary>
        /// Все хорошо
        /// </summary>
        Success = 100,

        /// <summary>
        /// Требует внимания
        /// </summary>
        Warning = 150,

        /// <summary>
        /// Необходимо принять меры
        /// </summary>
        Alarm = 200
    }
}
