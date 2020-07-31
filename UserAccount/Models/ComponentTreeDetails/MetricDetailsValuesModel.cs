using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.ComponentTreeDetails
{
    public class MetricDetailsValuesModel
    {
        public Guid Id { get; set; }

        public History[] Data { get; set; }

        public class History
        {
            public DateTime Date;

            public double? Value;

            public ObjectColor Color;

            public bool HasSignal;
        }
    }
}