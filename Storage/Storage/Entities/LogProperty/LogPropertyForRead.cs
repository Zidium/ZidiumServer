using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public class LogPropertyForRead
    {
        public LogPropertyForRead(Guid id, Guid logId, string name, DataType dataType, string value)
        {
            Id = id;
            LogId = logId;
            Name = name;
            DataType = dataType;
            Value = value;
        }

        public Guid Id { get; }

        public Guid LogId { get; }

        public string Name { get; }

        public DataType DataType { get; }

        public string Value { get; }
    }
}
