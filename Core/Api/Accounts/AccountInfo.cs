using System;
using Zidium.Core.ConfigDb;

namespace Zidium.Core.Api
{
    public class AccountInfo
    {
        public Guid Id { get; set; }

        public AccountType Type { get; set; }

        public Guid AccountDatabaseId { get; set; }

        public string DatabaseConnectionString { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public AccountStatus Status { get; set; }

        public DateTime? LastOverLimitDate { get; set; }

        public DateTime? OverlimitEmailDate { get; set; }

        public string SecretKey { get; set; }

        public Guid RootId { get; set; }

        public DateTime CreationDate { get; set; }

        public Guid? UserAgentTag { get; set; }
    }
}
