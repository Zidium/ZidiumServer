using System;
using Zidium.Core.ConfigDb;

namespace Zidium.Core.Api
{
    public class UpdateAccountRequestData
    {
        public Guid Id { get; set; }

        public AccountType? Type { get; set; }

        public string DisplayName { get; set; }

        public AccountStatus? Status { get; set; }

        public DateTime? LastOverLimitDate { get; set; }

        public bool ClearLastOverLimitDate { get; set; }

        public DateTime? OverlimitEmailDate { get; set; }

        public bool ClearOverlimitEmailDate { get; set; }

        public Guid? UserAgentTag { get; set; }
    }
}
