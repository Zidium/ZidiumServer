using System;

namespace Zidium.Core.AccountsDb
{
    public class Role
    {
        public Guid Id { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }
    }
}
