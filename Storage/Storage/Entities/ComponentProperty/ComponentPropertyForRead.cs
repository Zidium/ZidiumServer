using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public class ComponentPropertyForRead
    {
        public ComponentPropertyForRead(Guid id, Guid componentId, string name, string value, DataType dataType)
        {
            Id = id;
            ComponentId = componentId;
            Name = name;
            Value = value;
            DataType = dataType;
        }

        public Guid Id { get; }

        public Guid ComponentId { get; } 

        public string Name { get; }

        public string Value { get; }

        public DataType DataType { get; }

        public ComponentPropertyForUpdate GetForUpdate()
        {
            return new ComponentPropertyForUpdate(Id);
        }
    }
}
