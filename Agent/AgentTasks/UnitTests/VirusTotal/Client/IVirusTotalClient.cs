namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal.Client
{
    public interface IVirusTotalClient
    {
        ScanResponse Scan(ScanRequest request);

        ReportResponse Report(ReportRequest request);
    }
}
