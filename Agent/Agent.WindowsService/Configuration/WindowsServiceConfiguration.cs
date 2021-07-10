using Microsoft.Extensions.Configuration;
using Zidium.Common;

namespace Zidium.Agent
{
    public class WindowsServiceConfiguration : BaseConfiguration<WindowsServiceOptions>, IAgentWindowsServiceConfiguration
    {
        public WindowsServiceConfiguration(IConfiguration configuration) : base(configuration)
        {
        }

        public string ServiceName => Get().Install.ServiceName;

        public string ServiceDescription => Get().Install.ServiceDescription;
    }
}
