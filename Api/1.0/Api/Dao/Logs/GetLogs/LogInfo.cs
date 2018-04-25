using System;

namespace Zidium.Api
{
    public class LogInfo
    {
        public LogInfo()
        {
            Properties = new ExtentionPropertyCollection();
        }

        public Guid Id { get; set; }

        public LogLevel Level { get; set; }

        public DateTime Date { get; set; }

        public int Order { get; set; }

        public string Message { get; set; }

        public string Context { get; set; }

        public ExtentionPropertyCollection Properties { get; set; }
    }
}
