using Zidium.Storage;

namespace Zidium.UserAccount.Models.Events
{
    public class ShowEventModelReason
    {
        public EventForRead Event { get; set; }

        public EventTypeForRead EventType { get; set; }

        public ComponentForRead Component { get; set; }

        public MetricForRead Metric { get; set; }

        public UnitTestForRead UnitTest { get; set; }
    }
}