using System;
using System.Collections.Generic;
using Zidium.Storage;

namespace Zidium.Core.Api
{
    public class LogRow
    {
        public Guid Id { get; set; }

        public LogLevel Level { get; set; }

        public DateTime Date { get; set; }

        public int Order { get; set; }

        public string Message { get; set; }

        public string Context { get; set; }

        public List<ExtentionPropertyDto> Properties { get; set; }
    }
}
