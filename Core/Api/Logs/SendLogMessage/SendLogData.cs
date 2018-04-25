using System;
using System.Collections.Generic;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.Api
{
    public class SendLogData
    {
        public Guid? ComponentId { get; set; }

        public DateTime? Date { get; set; }

        public int? Order { get; set; }

        public LogLevel? Level { get; set; }

        public string Message { get; set; }

        public string Context { get; set; }

        public List<ExtentionPropertyDto> Properties { get; set; }

        public long GetSize()
        {
            long size = DataSizeHelper.DbLogRecordOverhead;
            if (Message != null)
            {
                size += Message.Length * sizeof(char);
            }
            if (Context != null)
            {
                size += Context.Length * sizeof(char);
            }
            if (Properties != null)
            {
                foreach (var property in Properties)
                {
                    if (property != null)
                    {
                        size += DataSizeHelper.DbLogParameterRecordOverhead + property.GetSize();
                    }
                }
            }
            return size;
        }
    }
}
