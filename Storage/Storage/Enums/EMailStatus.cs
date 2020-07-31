namespace Zidium.Storage
{
    public enum EmailStatus
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
