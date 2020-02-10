namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal.Client
{
    public class ReportRequest
    {
        public string Apikey { get; set; }

        public string ScanId { get; set; }

        public string Resource { get; set; }
    }
}
