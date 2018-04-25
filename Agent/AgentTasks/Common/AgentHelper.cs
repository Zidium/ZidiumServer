using Zidium.Core.Api.Dispatcher;

namespace Zidium.Agent.AgentTasks
{
    public static class AgentHelper
    {
        public static DispatcherClient GetDispatcherClient()
        {
            return new DispatcherClient("Agent");
        }

        public static string GetVersion()
        {
            return typeof (AgentService).Assembly.GetName().Version.ToString();
        }
    }
}
