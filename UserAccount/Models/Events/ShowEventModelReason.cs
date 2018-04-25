using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models.Events
{
    public class ShowEventModelReason
    {
        public Event Event { get; set; }

        public Component Component { get; set; }

        public Metric Metric { get; set; }

        public UnitTest UnitTest { get; set; }
    }
}