using System;

namespace Zidium.Storage
{
    public class LogForAdd
    {
        /// <summary>
        /// ИД записи
        /// </summary>
        public Guid Id;

        /// <summary>
        /// Ссылка на компонент
        /// </summary>
        public Guid ComponentId;

        /// <summary>
        /// Важность
        /// </summary>
        public LogLevel Level;

        /// <summary>
        /// Дата и время
        /// </summary>
        public DateTime Date;

        /// <summary>
        /// Номер по порядку в пределах секунды
        /// </summary>
        public int Order;

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message;

        /// <summary>
        /// Количество расширенных свойств
        /// </summary>
        public int ParametersCount;

        /// <summary>
        /// Контекст
        /// </summary>
        public string Context;

        /// <summary>
        /// Параметры
        /// </summary>
        public LogPropertyForAdd[] Properties;

    }
}
