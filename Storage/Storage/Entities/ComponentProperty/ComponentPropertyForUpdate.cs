using System;

namespace Zidium.Storage
{
    public class ComponentPropertyForUpdate
    {
        public ComponentPropertyForUpdate(Guid id)
        {
            Id = id;
            Name = new ChangeTracker<string>();
            Value = new ChangeTracker<string>();
            DataType = new ChangeTracker<DataType>();
        }

        public Guid Id { get; }

        public ChangeTracker<string> Name { get; }

        public ChangeTracker<string> Value { get; }

        public ChangeTracker<DataType> DataType { get; }
    }
}