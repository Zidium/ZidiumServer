namespace Zidium.Agent.AgentTasks.UnitTests.TcpPortChecks
{
    public enum TcpPortCheckCode
    {
        Opened = 1,
        Closed = 2,
        NoIp = 3,
        InvalidPort = 4,
        UnknownError = 5
    }
}
