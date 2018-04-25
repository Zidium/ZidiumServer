namespace Zidium.Api
{
    /// <summary>
    /// Инициализирует свойства события.
    /// В основном используется для установки дополнительных свойств события в веб-приложении (IP, login и прочее)
    /// </summary>
    public interface IEventPreparer
    {
        /// <summary>
        /// Инициализирует свойства события (в основном доп. свойства)
        /// </summary>
        /// <param name="eventObj">событие</param>
        void Prepare(SendEventBase eventObj);
    }
}
