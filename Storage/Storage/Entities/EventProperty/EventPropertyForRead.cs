using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    // Параметр события
    public class EventPropertyForRead
    {
        public EventPropertyForRead(Guid id, Guid eventId, string name, string value, DataType dataType)
        {
            Id = id;
            EventId = eventId;
            Name = name;
            Value = value;
            DataType = dataType;
        }

        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Ссылка на событие
        /// </summary>
        public Guid EventId { get; }

        /// <summary>
        /// Имя параметра
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Значение параметра
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Тип данных
        /// </summary>
        public DataType DataType { get; }

    }
}
