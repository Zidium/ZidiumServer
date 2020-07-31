using System;

namespace Zidium.Storage.Ef
{
    // Параметр события
    public class DbEventProperty
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Ссылка на событие
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Событие
        /// </summary>
        public virtual DbEvent Event { get; set; }

        /// <summary>
        /// Имя параметра
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Значение параметра
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Тип данных
        /// </summary>
        public DataType DataType { get; set; }

    }
}
