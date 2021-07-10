using System;

namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal
{
    public class VirusTotalProcessorReport
    {
        public int Total { get; set; }

        public int Positives { get; set; }

        public ScanItem[] Scans { get; set; }

        public DateTime ScanDate { get; set; }

        public string Permalink { get; set; }

        public class ScanItem
        {
            public string Name { get; set; }

            public bool Detected { get; set; }

            public string Message { get; set; }
        }
    }
}
