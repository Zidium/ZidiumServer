using System;
using Zidium.Core.AccountsDb;
using Zidium.UserAccount.Models.ExtentionProperties;

namespace Zidium.UserAccount.Models
{
    public class EventTypeLastEventModel
    {
        public Guid Id { get; set; }

        public DateTime EndDate { get; set; }

        public Component Component { get; set; }

        public UnitTest Unittest { get; set; }

        public Metric Metric { get; set; }

        public string Message { get; set; }

        public ExtentionPropertiesModel Properties { get; set; }
    }
}