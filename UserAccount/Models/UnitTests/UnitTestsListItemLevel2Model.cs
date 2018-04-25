using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models
{
    public class UnitTestsListItemLevel2Model
    {
        public Guid Id { get; set; }

        public DateTime? Date { get; set; }

        public Component Component { get; set; }

        public string DisplayName { get; set; }

        public string Message { get; set; }

        public MonitoringStatus Result { get; set; }

        public bool HasBanner { get; set; }
    }
}