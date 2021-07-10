namespace Zidium.Api.Dto
{
    public enum SendEventCategory
    {
        /// <summary>
        /// Обычные (общие) события компонента
        /// </summary>
        ComponentEvent = EventCategory.ComponentEvent,

        /// <summary>
        /// Ошибки приложений
        /// </summary>
        ApplicationError = EventCategory.ApplicationError
    }
}
