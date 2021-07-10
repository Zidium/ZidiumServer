using System;
using Zidium.Api.Dto;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal
{
    public class VirusTotalProcessorOutputData
    {
        public VirusTotalStep NextStep { get; set; }

        public string ScanId { get; set; }

        public DateTime? ScanTime { get; set; }

        public DateTime? NextStepProcessTime { get; set; }

        public SendUnitTestResultRequestDataDto Result { get; set; }

        public VirusTotalErrorCode? ErrorCode { get; set; }
    }
}
