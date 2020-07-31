using System;
using Zidium.Core.Api;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal
{
    public class VirusTotalProcessorOutputData
    {
        public VirusTotalStep NextStep { get; set; }

        public string ScanId { get; set; }

        public DateTime? ScanTime { get; set; }

        public DateTime? NextStepProcessTime { get; set; }

        public SendUnitTestResultRequestData Result { get; set; }

        public VirusTotalErrorCode? ErrorCode { get; set; }
    }
}
