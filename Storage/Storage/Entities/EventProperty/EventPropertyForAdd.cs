using System;

namespace Zidium.Storage
{
    public class EventPropertyForAdd
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id;

        /// <summary>
        /// Ссылка на событие
        /// </summary>
        public Guid EventId;

        /// <summary>
        /// Имя параметра
        /// </summary>
        public string Name;

        /// <summary>
        /// Значение параметра
        /// </summary>
        public string Value;

        /// <summary>
        /// Тип данных
        /// </summary>
        public DataType DataType;

    }
}
