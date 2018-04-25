using System;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    // Параметр события
    public class EventProperty
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
        public virtual Event Event { get; set; }

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

        public bool HasValue
        {
            get { return Value != null; }
        }
    }
}
