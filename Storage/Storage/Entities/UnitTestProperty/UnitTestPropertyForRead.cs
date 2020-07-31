using System;

namespace Zidium.Storage
{
    public class UnitTestPropertyForRead
    {
        public UnitTestPropertyForRead(
            Guid id, 
            Guid unitTestId, 
            string name, 
            string value, 
            DataType dataType)
        {
            Id = id;
            UnitTestId = unitTestId;
            Name = name;
            Value = value;
            DataType = dataType;
        }

        public Guid Id { get; }

        public Guid UnitTestId { get; }

        public string Name { get; }

        public string Value { get; }

        public DataType DataType { get; }

    }
}
