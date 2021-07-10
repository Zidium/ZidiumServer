namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal.Client
{
    public class ScanResponse : ResponseBase
    {
        public string scan_id { get; set; }
        public string scan_date { get; set; }
        public string url { get; set; }
        public string permalink { get; set; }
    }
}
