using System;

namespace Zidium.Core.Api
{
    public class DatabaseInfo 
    {
        public Guid Id;

        public string SystemName;

        public string DisplayName;

        public string Description;

        public string ConnectionString;

        public bool IsBroken;
    }
}
