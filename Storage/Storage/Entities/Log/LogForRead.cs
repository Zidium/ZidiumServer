using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Сообщение лога
    /// </summary>
    public class LogForRead
    {
        public LogForRead(
            Guid id, 
            Guid componentId, 
            LogLevel level, 
            DateTime date, 
            int order, 
            string message, 
            int parametersCount, 
            string context)
        {
            Id = id;
            ComponentId = componentId;
            Level = level;
            Date = date;
            Order = order;
            Message = message;
            ParametersCount = parametersCount;
            Context = context;
        }

        /// <summary>
        /// ИД записи
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Ссылка на компонент
        /// </summary>
        public Guid ComponentId { get; }

        /// <summary>
        /// Важность
        /// </summary>
        public LogLevel Level { get; }

        /// <summary>
        /// Дата и время
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        /// Номер по порядку в пределах секунды
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Количество расширенных свойств
        /// </summary>
        public int ParametersCount { get; }

        /// <summary>
        /// Контекст
        /// </summary>
        public string Context { get; }

    }
}
