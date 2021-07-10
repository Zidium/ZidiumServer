using Microsoft.Extensions.Configuration;
using Zidium.Common;
using Zidium.Core;

namespace Zidium.Dispatcher
{
    internal class Configuration : BaseConfiguration<Options>,
        IDebugConfiguration, IDatabaseConfiguration, ILogicConfiguration, IAccessConfiguration
    {
        public Configuration(IConfiguration configuration) : base(configuration)
        {
        }

        public bool DebugMode => Get().DebugMode;

        public string ProviderName => Get().Database.ProviderName;

        public string ConnectionString => Get().Database.ConnectionString;

        public string WebSite => Get().WebSite;

        public string SecretKey => Get().SecretKey;

        public string MasterPassword => null;

        public int? EventsMaxDays => Get().EventsMaxDays;

        public int? LogMaxDays => Get().LogMaxDays;

        public int? MetricsMaxDays => Get().MetricsMaxDays;

        public int? UnitTestsMaxDays => Get().UnitTestsMaxDays;
    }
}