using System;

namespace Zidium.Core.Common
{
    public class ReportPeriodRange
    {
        public ReportPeriod Period { get; set; }

        public DateTime From { get; set; }  

        public DateTime To { get; set; }

        public TimeSpan Duration
        {
            get { return To - From; }
        }
    }
}
