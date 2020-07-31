using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.ComponentTreeDetails
{
    public class ComponentDetailsLogModel
    {
        public class LogMessage
        {
            public Guid Id { get; set; }

            public DateTime Time { get; set; }

            public LogLevel Level { get; set; }

            public string Message { get; set; }

            public bool HasProperties { get; set; }
        }

        public Guid ComponentId { get; set; }

        public int Count { get; set; }

        public LogLevel Level { get; set; }

        public LogMessage[] Messages { get; set; }
    }
}