using System;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal.Processor
{
    public class VirusTotalProcessorInputData
    {
        public string ApiKey { get; set; }

        public string Url { get; set; }

        public DateTime? ScanTime { get; set; }

        public string ScanId { get; set; }

        public VirusTotalStep NextStep { get; set; }

        /// <summary>
        /// Номер попытки
        /// </summary>
        public int AttempCount { get; set; }
    }
}
