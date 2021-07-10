using System;
using System.Collections.Generic;

namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal.Client
{
    public class ReportResponse : ResponseBase
    {
        public string scan_id { get; set; }

        public string resource { get; set; }

        public string url { get; set; }

        public string scan_date { get; set; }

        public string filescan_id { get; set; }

        public Int32? positives { get; set; }

        public Int32? total { get; set; }

        public string permalink { get; set; }

        public Dictionary<string, ScanItem> scans { get; set; }

        public class ScanItem
        {
            public bool? detected { get; set; }

            public string result { get; set; }
        }
    }
}
