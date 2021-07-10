using System;

namespace Zidium.Storage
{
    public class RoleForRead
    {
        public RoleForRead(Guid id, string systemName, string displayName)
        {
            Id = id;
            SystemName = systemName;
            DisplayName = displayName;
        }

        public Guid Id { get; }

        public string SystemName { get; }

        public string DisplayName { get; }
    }
}
