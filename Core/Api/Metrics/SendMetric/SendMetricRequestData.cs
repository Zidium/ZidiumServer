using System;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.Api
{
    public class SendMetricRequestData
    {
        public Guid? ComponentId { get; set; }

        public string Name { get; set; }

        public double? Value { get; set; }

        public double? ActualIntervalSecs { get; set; }

        // TODO Move to extension type
        public Int64 GetSize()
        {
            return DataSizeHelper.DbMetricRecordOverhead;
        }
    }
}
