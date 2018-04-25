using System;

namespace Zidium.Core.Api
{
    public class DatabaseInfo 
    {
        public Guid Id { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string ConnectionString { get; set; }

        public bool IsBroken { get; set; }
    }
}
