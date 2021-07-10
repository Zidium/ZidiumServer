using System;

namespace Zidium.Storage.Ef
{
    public class DbRole
    {
        public Guid Id { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }
    }
}
