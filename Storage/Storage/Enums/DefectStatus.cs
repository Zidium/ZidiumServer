namespace Zidium.Storage
{
    /// <summary>
    /// Статус дефекта
    /// </summary>
    public enum DefectStatus
    {
        /// <summary>
        /// Для служебных целей. Не должен никогда появиться.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Дефект открыт в первый  раз
        /// </summary>
        Open = 1,

        /// <summary>
        /// Открыт повторно (дефект вновь появился в новой версии)
        /// </summary>
        Reopened = 2,

        /// <summary>
        /// Назначен исполнитель
        /// </summary>
        // Assigned = 3, решил удалить, т.к. после повторного открытия, ответственный будет тот же

        /// <summary>
        /// Исполнитель взял дефект в работу
        /// </summary>
        InProgress = 4,

        /// <summary>
        /// Тестировщик проверяет, что дефект исправлен
        /// </summary>
        Testing = 5,

        /// <summary>
        /// Дефект закрыт
        /// </summary>
        Closed = 6
    }
}
