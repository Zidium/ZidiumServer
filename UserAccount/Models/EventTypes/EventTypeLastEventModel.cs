using System;
using Zidium.Storage;
using Zidium.UserAccount.Models.ExtentionProperties;

namespace Zidium.UserAccount.Models
{
    public class EventTypeLastEventModel
    {
        public Guid Id { get; set; }

        public DateTime EndDate { get; set; }

        public ComponentForRead Component { get; set; }

        public UnitTestForRead Unittest { get; set; }

        public MetricForRead Metric { get; set; }

        public MetricTypeForRead MetricType { get; set; }

        public string Message { get; set; }

        public ExtentionPropertiesModel Properties { get; set; }
    }
}