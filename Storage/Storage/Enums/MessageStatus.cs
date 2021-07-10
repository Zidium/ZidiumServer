namespace Zidium.Storage
{
    public enum MessageStatus
    {
        /// <summary>
        /// Ожидает отправки
        /// </summary>
        InQueue = 1,

        /// <summary>
        /// Успешно отправлено
        /// </summary>
        Sent = 2,

        /// <summary>
        /// Ошибка отправки
        /// </summary>
        Error = 3

    }
}